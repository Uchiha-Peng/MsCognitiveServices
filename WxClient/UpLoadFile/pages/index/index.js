Page({

  /**
   * 页面的初始数据
   */
  data: {
    FileName: "",
    FilePath: "点我选择文件！"
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {

  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function () {

  },
  ChoseFile: function () {
    var that = this;
    wx.chooseImage({
      count: 1, // 默认9
      sizeType: ['original', 'compressed'], // 可以指定是原图还是压缩图，默认二者都有
      sourceType: ['album', 'camera'], // 可以指定来源是相册还是相机，默认二者都有
      success: function (res) {
        // 返回选定照片的本地文件路径列表，tempFilePath可以作为img标签的src属性显示图片
        that.setData({ FilePath: res.tempFilePaths[0] })
        console.info(res);
      },
      fail: function (res) {
        wx.showToast({
          title: '文件选择失败!',
        })
      }
    })
  },
  formSubmit: function () {
    var that = this;
    var path = that.data.FilePath;
    console.info(path);
    if (path.includes(".jpg") || path.includes(".png") || path.includes(".jpeg")) {
      wx.uploadFile({
        url: 'http://localhost:8432/api/Services/GetImageByteArry',
        filePath: path,
        name: 'file',
        formData: {
          'FileName': that.data.FileName
        },
        success: function (res) {
          console.info(res);
          var data = res.data
        },
        fail: function (res) {
          wx.showToast({
            title: '上传失败!',
          })
        }
      })
    }
  },
  InputName: function (e) {
    this.setData({ FileName: e.detail.value })
    console.info(this.data.FileName);
  }
})