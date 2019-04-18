﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Atlantis.Rabbit;
using Atlantis.Rabbit.Utilies;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RabbitConfigurationExetension
    {
        public static IServiceProvider ServiceProvider;

        public static IServiceCollection AddRabbit(
            this IServiceCollection services,
            Action<RabbitBuilder> builderAction)
        {
            var builder=new RabbitBuilder();
            builderAction(builder);
            
            services.AddSingleton<IRabbitMessagePublisher, RabbitMessagePublisher>();
            if(builder.ServerOptions==null||builder.ServerOptions==null)
            {
                new ArgumentNullException("Please setting rabbit!");
            }

            var assemblies=new List<Assembly>();
            foreach(var item in builder.ScanAssemblies)
            {
                assemblies.Add(Assembly.Load(item));
            }
            
            var types = RefelectionHelper.GetImplInterfaceTypes(
                typeof(IRabbitMessagingHandler), false, assemblies.ToArray());
            foreach (var type in types)
            {
                if (type.IsAbstract) continue;
                services.AddScoped(type);
                builder.Metadatas.Add(type);
            }
            services.AddSingleton(builder);
            services.AddHostedService<RabbitHostService>();
            ServiceProvider=services.BuildServiceProvider();
            
            return services;

        }

        // public static Configuration RegisterRabbitMQ(this Configuration configuration, string rabbitMQSettingConfigName)
        // {
        //     ObjectContainer.Register<IRabbitMessagePublisher, RabbitMessagePublisher>();
        //     configuration.Setting.SetRabbitMQSetting(configuration.GetSetting<RabbitMQSetting>(rabbitMQSettingConfigName) ?? throw new ArgumentNullException("Please setting rabbit!"));
        //     var types = RefelectionHelper.GetImplInterfaceTypes(typeof(IRabbitMessagingHandler), false, configuration.Setting.BussinessAssemblies);
        //     foreach (var type in types)
        //     {
        //         if (type.IsAbstract) continue;
        //         ObjectContainer.Register(type, LifeScope.Transient);
        //         _metadatas.Add(type);
        //     }
        //     return configuration;


        // }

        // public static Configuration StartRabbitMQ(this Configuration configuration)
        // {
        //     var setting = configuration.Setting.GetRabbitMQSetting();
        //     foreach (var type in _metadatas)
        //     {
        //         var instance = (IRabbitMessagingHandler)ObjectContainer.Resolve(type);
        //         if (!instance.IsEnable)
        //         {
        //             continue;
        //         }
        //         if (IsStop(setting, instance.Queue))
        //         {
        //             Log.Warn($"The queue has stop in the config. queue({instance.Queue})");
        //             continue;
        //         }
        //         if (string.IsNullOrWhiteSpace(instance.Name))
        //         {
        //             throw new ArgumentNullException($"The virtualhost is null in the handler({instance.GetType().Name})!");
        //         }
        //         var hostSetting = setting.GetServer(instance.Name) ?? throw new ArgumentNullException($"The handler name({instance.Name}) cannot found setting!");
        //         var rabbitContext = new RabbitConnection(instance.Queue, instance.Exchange, instance.RoutingKey, hostSetting, instance.Handle);
        //         Task.Run(() => { rabbitContext.Start(); });
        //     }
        //     _metadatas = null;
        //     return configuration;
        // }

        // private static bool IsStop(RabbitMQSetting setting, string queueName)
        // {
        //     if (setting == null ||
        //        setting.StopQueues == null ||
        //        setting.StopQueues.Length == 0)
        //     {
        //         return false;
        //     }

        //     if (setting.StopQueues.Contains(queueName))
        //     {
        //         return true;
        //     }
        //     else
        //     {
        //         return false;
        //     }
        // }
    }

    // public static class RabbitMQConfigurationSettingExtension
    // {
    //     private static RabbitMQSetting _setting;

    //     public static RabbitMQSetting GetRabbitMQSetting(this ConfigurationSetting setting)
    //     {
    //         return _setting;
    //     }

    //     public static ConfigurationSetting SetRabbitMQSetting(this ConfigurationSetting configurationSetting, RabbitMQSetting rabbitMQSetting)
    //     {
    //         _setting = rabbitMQSetting;
    //         return configurationSetting;
    //     }
    // }
}