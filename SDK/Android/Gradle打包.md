## Android使用Gradle命令行(编译)出包

### Gradle简介

Gradle是一个基于Apache Ant和Apache Maven概念的项目自动化建构工具。Gradle 构建脚本使用的是 Groovy 或 Kotlin 的特定领域语言来编写的，而不是传统的XML。

### 下载和配置

Gradle下载：https://gradle.org/releases/

需要把gradle解压后的bin路径配置到环境变量的Path中。

### 命令行生成APK

gradle --warning-mode all 可以打印出当前gradle存在的所有警告信息

gradle clean 清理gradle build缓存和gradle下载的一些依赖

gradle assembleDebug 在~\launcher\build\Output\apk\debug中生成debug签名的apk

gradle assembleRelease 在~\launcher\build\Output\apk\release中生成Release签名的apk

(检查签名信息配置是否正确~\launcher\build.gradle)















