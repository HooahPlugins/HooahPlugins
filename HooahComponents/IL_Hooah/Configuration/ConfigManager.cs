#if AI || HS2
using System;
using System.Collections.Generic;
using HooahUtility.Controller.ContentManagers;
using HooahUtility.Model;
using MessagePack;
#endif

namespace HooahComponents.Configuration
{
#if AI || HS2
    public abstract class ConfigManager<T, TConfig> : IConfigManager where T : ConfigManager<T, TConfig>
        where TConfig : IFormData, new()
    {
        private static readonly Dictionary<Type, T> SingletonInstances = new Dictionary<Type, T>();
        public static T Instance => SingletonInstances.TryGetValue(typeof(T), out var i) ? i : null;
        public TConfig Config = new TConfig();

        protected ConfigManager()
        {
            SingletonInstances[typeof(T)] = this as T;
        }

        public virtual void PreSerializeConfig()
        {
        }

        public virtual void PreDeserializeConfig()
        {
        }

        public virtual void PostDeserializeConfig()
        {
        }

        public byte[] SerializeConfig()
        {
            PreSerializeConfig();
            return MessagePackSerializer.Serialize(Config); // Serialize<TConfig>(Config)
        }

        public void DeserializeConfig(byte[] messagePackByteArray)
        {
            PreDeserializeConfig();
            Config = MessagePackSerializer.Deserialize<TConfig>(messagePackByteArray);
            PostDeserializeConfig();
        }

        public void AddForm(SerializedDataForm form, Action pre = null, Action post = null)
        {
            typeof(TConfig).AddForms(form, new object[] {Instance.Config}, pre, post);
        }
    }
#endif
}