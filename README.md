[![Yukikoi Melt](https://images2.imgbox.com/0b/df/66PTpxUz_o.png)](https://t.me/Yukikoi_Melt)
[![GitHub Official Release](https://img.shields.io/github/release/ppy/osu.svg)](https://github.com/ppy/osu/releases/latest)
[![Crowdin](https://d322cqt584bo4o.cloudfront.net/osu-web/localized.svg)](https://crowdin.com/project/osu-web)

节奏只需*点击一下*即可！

点击[此处](https://osu.ppy.sh)访问官方网站owo

<p align="center">
<img width="500" alt="osu! logo" src="assets/lazer.png">
</p>

好大一个osu(

### 下载源码

克隆存储库:

```shell
git clone https://github.com/Galcatqwq/Yukikoi-lazer
cd Yukikoi-lazer
```

#### 通过 CLI 构建

你可以通过以下命令构建`osu`:

```shell
dotnet run --project osu.Desktop
```

如果你打算本地进行一些性能测试/或者是为了直接开O, 请添加 `-c Release`参数到构建指令中,`Debug`构建出的版本会有很严重的性能问题.

如果出现构建错误,可以尝试使用`dotnet restore`恢复NuGet包.

### 利用 osu-resource 和 osu-framework 进行测试

如果你打算将本存储库与 [osu-resources](https://github.com/ppy/osu-resources) or [osu-framework](https://github.com/ppy/osu-framework) 进行交叉测试,可以使用下面的脚本:

Windows:

```ps
UseLocalFramework.ps1
UseLocalResources.ps1
```

macOS / Linux:

```ps
UseLocalFramework.sh
UseLocalResources.sh
```

顺带一提,执行前先检查本地有 osu-resource 和 osu-framework 的目录没:P :

```
|- osu            // 此存储库
|- osu-framework
|- osu-resources
