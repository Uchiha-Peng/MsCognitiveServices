using System;
using System.Collections.Generic;
using System.Text;

namespace CognitiveServices
{
    public class FaceInfo
    {
        /// <summary>
        /// 检测到的脸部的唯一faceId
        /// </summary>
        public string faceId { get; set; }

        /// <summary>
        /// 图像上面部位置的矩形区域
        /// </summary>
        public FaceRectangle faceRectangle { get; set; }

        /// <summary>
        /// 面部属性
        /// </summary>
        public FaceAttributes faceAttributes { get; set; }
    }

}

/// <summary>
/// 图像上面部位置的矩形区域
/// </summary>
public class FaceRectangle
{
    /// <summary>
    /// 顶部
    /// </summary>
    public int top { get; set; }

    /// <summary>
    /// 左边
    /// </summary>
    public int left { get; set; }

    /// <summary>
    /// 宽度
    /// </summary>
    public int width { get; set; }

    /// <summary>
    /// 高度
    /// </summary>
    public int height { get; set; }
}

/// <summary>
///头部坐标
/// </summary>
public class HeadPose
{
    /// <summary>
    /// 间距
    /// </summary>
    public double pitch { get; set; }

    /// <summary>
    /// 滚动
    /// </summary>
    public double roll { get; set; }

    /// <summary>
    /// 偏转
    /// </summary>
    public double yaw { get; set; }
}

public class FacialHair
{
    public double moustache { get; set; }
    public double beard { get; set; }
    public double sideburns { get; set; }
}

/// <summary>
/// 情绪
/// </summary>
public class Emotion
{
    /// <summary>
    /// 愤怒
    /// </summary>
    public double anger { get; set; }

    /// <summary>
    /// 鄙视
    /// </summary>
    public double contempt { get; set; }

    /// <summary>
    /// 厌恶
    /// </summary>
    public double disgust { get; set; }

    /// <summary>
    /// 恐惧
    /// </summary>
    public double fear { get; set; }


    /// <summary>
    /// 幸福
    /// </summary>
    public double happiness { get; set; }

    /// <summary>
    /// 自然
    /// </summary>
    public double neutral { get; set; }

    /// <summary>
    /// 伤心
    /// </summary>
    public double sadness { get; set; }

    /// <summary>
    /// 惊讶
    /// </summary>
    public double surprise { get; set; }
}

/// <summary>
/// 模糊
/// </summary>
public class Blur
{
    /// <summary>
    /// 模糊级别
    /// </summary>
    public string blurLevel { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public double value { get; set; }
}

/// <summary>
/// 曝光
/// </summary>
public class Exposure
{
    /// <summary>
    /// 曝光级别
    /// </summary>
    public string exposureLevel { get; set; }

    /// <summary>
    /// 曝光值
    /// </summary>
    public double value { get; set; }
}

/// <summary>
/// 噪点
/// </summary>
public class Noise
{
    /// <summary>
    /// 噪点级别
    /// </summary>
    public string noiseLevel { get; set; }

    /// <summary>
    /// 噪点值
    /// </summary>
    public double value { get; set; }
}

/// <summary>
/// 化妆
/// </summary>
public class Makeup
{
    /// <summary>
    /// 眼妆
    /// </summary>
    public bool eyeMakeup { get; set; }

    /// <summary>
    /// 唇妆
    /// </summary>
    public bool lipMakeup { get; set; }
}

/// <summary>
/// 装饰物
/// </summary>
public class Accessory
{
    /// <summary>
    /// 装饰物类型
    /// </summary>
    public string type { get; set; }

    /// <summary>
    /// 自信度
    /// </summary>
    public double confidence { get; set; }
}

/// <summary>
/// 遮挡
/// </summary>
public class Occlusion
{
    /// <summary>
    /// 前额遮挡
    /// </summary>
    public bool foreheadOccluded { get; set; }

    /// <summary>
    /// 眼睛遮挡
    /// </summary>
    public bool eyeOccluded { get; set; }

    /// <summary>
    /// 嘴部遮挡
    /// </summary>
    public bool mouthOccluded { get; set; }
}

/// <summary>
/// 头发颜色
/// </summary>
public class HairColor
{
    /// <summary>
    /// 头发颜色
    /// </summary>
    public string color { get; set; }

    /// <summary>
    /// 自信度
    /// </summary>
    public double confidence { get; set; }
}

/// <summary>
/// 头发
/// </summary>
public class Hair
{
    /// <summary>
    /// 头发特征
    /// </summary>
    public double bald { get; set; }

    /// <summary>
    /// 是否可见
    /// </summary>
    public bool invisible { get; set; }

    /// <summary>
    /// 头发颜色
    /// </summary>
    public List<HairColor> hairColor { get; set; }
}

public class FaceAttributes
{
    /// <summary>
    /// 微笑
    /// </summary>
    public double smile { get; set; }

    /// <summary>
    /// 摆造型
    /// </summary>
    public HeadPose headPose { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public string gender { get; set; }

    /// <summary>
    /// 年龄
    /// </summary>
    public double age { get; set; }

    /// <summary>
    /// 胡子
    /// </summary>
    public FacialHair facialHair { get; set; }

    /// <summary>
    /// 眼睛
    /// </summary>
    public string glasses { get; set; }

    /// <summary>
    /// 情绪
    /// </summary>
    public Emotion emotion { get; set; }

    /// <summary>
    /// 模糊
    /// </summary>
    public Blur blur { get; set; }

    /// <summary>
    /// 曝光
    /// </summary>
    public Exposure exposure { get; set; }

    /// <summary>
    /// 噪点
    /// </summary>
    public Noise noise { get; set; }

    /// <summary>
    /// 化妆
    /// </summary>
    public Makeup makeup { get; set; }

    /// <summary>
    /// 饰品
    /// </summary>
    public List<Accessory> accessories { get; set; }

    /// <summary>
    /// 遮挡
    /// </summary>
    public Occlusion occlusion { get; set; }

    /// <summary>
    /// 头发
    /// </summary>
    public Hair hair { get; set; }
}

