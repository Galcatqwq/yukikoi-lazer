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

### 构建

#### 通过 IDE 构建

You should load the solution via one of the platform-specific `.slnf` files, rather than the main `.sln`. This will reduce dependencies and hide platforms that you don't care about. Valid `.slnf` files are:

- `osu.Desktop.slnf` (most common)
- `osu.Android.slnf`
- `osu.iOS.slnf`

Run configurations for the recommended IDEs (listed above) are included. You should use the provided Build/Run functionality of your IDE to get things going. When testing or building new components, it's highly encouraged you use the `osu! (Tests)` project/configuration. More information on this is provided [below](#contributing).

To build for mobile platforms, you will likely need to run `sudo dotnet workload restore` if you haven't done so previously. This will install Android/iOS tooling required to complete the build.


#### 通过 CLI 构建

你可以通过以下命令构建`osu`:

```shell
dotnet run --project osu.Desktop
```

如果你打算本地进行一些性能测试/或者是为了直接开O, 请添加 `-c Release`参数到构建指令中,`Debug`构建出的版本会有严重的性能问题.

如果出现构建错误,可以尝试使用`dotnet restore`恢复NuGet包.
