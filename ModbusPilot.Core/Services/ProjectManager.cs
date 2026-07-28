using System;
using System.Collections.Generic;
using System.IO;
using ModbusPilot.Core.Models;
using Newtonsoft.Json;

namespace ModbusPilot.Core.Services
{
    public static class ProjectManager
    {
        // 保存项目
        public static void SaveProject(string filePath, List<ChannelConfig> channels, List<DashboardWidgetConfig> layout)
        {
            // 1. 构建根对象
            var profile = new ProjectProfile
            {
                Version = "0.9",
                LastModified = DateTime.Now,
                ProjectName = Path.GetFileNameWithoutExtension(filePath),
                Channels = channels,
                DashboardLayout = layout
            };

            // 2. 序列化配置
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented, // 格式化缩进，方便人眼阅读
                NullValueHandling = NullValueHandling.Ignore, // 忽略空值减小体积
                // 防止将来如果有循环引用导致崩溃
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            string json = JsonConvert.SerializeObject(profile, settings);

            // 3. 写入文件
            File.WriteAllText(filePath, json);
        }

        // 加载项目
        public static ProjectProfile LoadProject(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException("文件不存在", filePath);

            string json = File.ReadAllText(filePath);

            try
            {
                var profile = JsonConvert.DeserializeObject<ProjectProfile>(json);
                if (profile == null) throw new Exception("文件格式错误或内容为空");

                // 兼容性处理：如果 Channels 为空，初始化它
                if (profile.Channels == null) profile.Channels = new List<ChannelConfig>();

                return profile;
            }
            catch (Exception ex)
            {
                throw new Exception("项目加载失败: " + ex.Message);
            }
        }
    }
}