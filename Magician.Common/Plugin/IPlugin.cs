namespace Magician.Common.Plugin
{
    /// <summary>
    /// IPlugin 插件接口 
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// OnLoading 生命周期回调，当插件加载完毕被调用。可以从PluginUtil获取主应用传递的参数来初始化插件
        /// </summary>
        void OnLoading();

        /// <summary>
        /// BeforeTerminating 生命周期回调，卸载插件前调用
        /// </summary>
        void BeforeTerminating();

        /// <summary>
        /// Enabled 插件是否启用
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// PluginKey 插件关键字，不同的插件其Key是不一样的
        /// </summary>
        int PluginKey { get; }

        /// <summary>
        /// ServiceName 插件提供的服务的名字	
        /// </summary>
        string PluginName { get; }

        /// <summary>
        /// Description 插件的描述信息	
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Version 插件版本
        /// </summary>
        string Version { get; }
    }
}