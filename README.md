[![Yukikoi Melt](https://images2.imgbox.com/0b/df/66PTpxUz_o.png)](https://t.me/Yukikoi_Melt)
[![GitHub Official Release](https://img.shields.io/github/release/ppy/osu.svg)](https://github.com/ppy/osu/releases/latest)
[![Crowdin](https://d322cqt584bo4o.cloudfront.net/osu-web/localized.svg)](https://crowdin.com/project/osu-web)

节奏只需*点击一下*即可！

点击[此处](https://osu.ppy.sh)访问官方网站owo

<p align="center">
<img width="500" alt="osu! logo" src="assets/lazer.png">
</p>

好大一个osu(


### 构建

#### 通过 IDE 构建

您应该通过平台特定的 .slnf 文件（而不是主 .sln 文件）加载解决方案.这将减少依赖项并隐藏您不需要关注的平台.有效的 .slnf 文件包括：

osu.Desktop.slnf（最常用）
osu.Android.slnf
osu.iOS.slnf

已为推荐 IDE（如上所列）配置了运行预设.您应该使用 IDE 提供的 生成/运行 功能来启动项目

若需为移动平台构建，在首次操作前可能需要运行以下命令：

```shell
sudo dotnet workload restore 
``` 
这将安装完成构建所需的 Android/iOS 工具链。


#### 通过 CLI 构建

你可以通过以下命令构建`osu`:

```shell
dotnet run --project osu.Desktop
```

添加 `-c Release`参数到构建指令中,`Debug`构建出的版本会有严重的性能问题.

如果出现构建错误,可以尝试使用`dotnet restore`恢复NuGet包.
