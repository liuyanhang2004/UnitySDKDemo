#!/bin/bash
: <<!
需要有全局的Gradle环境和导出Android项目使用的Android Gradle Plugin Version版本匹配！
版本对照表：https://docs.unity3d.com/Manual/android-gradle-overview.html
!

LANG=C.UTF-8
# Android项目根目录
buildDir=$1
# Apk保存目录
outDir=$2
# 运行到设备需要保证设备连接并正确配置了启动Activity
isRunToDevice=$3
# log path
buildLogPath=$outDir/BuildLog.txt
date=$(date "+%Y-%m-%d %H:%M:%S")

main() {
    # Init
    if [ -z "$buildDir" ] || [ -z "$outDir" ]; then
        log "参数不完整" false
        log "BuildDir=$buildDir" false
        log "OutDir=$outDir" false
        exit
    fi

    # 清理输出目录
    if ! hasDir "$outDir"; then
        mkdir -p "$outDir"
    else
        rm -rf "${outDir:?}/"*
    fi

    # Start
    log "Build At $date"
    if ! hasDir "$buildDir"; then
        log "BuildDir ${buildDir} 不存在"
        buildFail
    fi
    cd "$buildDir" || buildFail

    # Gradle
    gradleHandler

    # Check Build Result
    buildGenerateDir="$buildDir/launcher/build/outputs/apk/release"
    if ! hasApk "$buildGenerateDir"; then
        buildFail
    fi

    # Move File
    cp -r "$buildGenerateDir/." "$outDir" >>"$buildLogPath"
    
    log "Build Success" false
    log "OutDir: $outDir" false
    
    # RunToDevice
    if [ "$isRunToDevice" = "true" ] || [ "$isRunToDevice" = "True" ]; then
        runToDevice
    fi
}

runToDevice() {
    log "------RunToDevice------"
    # 连接mumu模拟器
    adb connect 127.0.0.1:16384 >>"$buildLogPath" 2>&1
    # 查看所有设备|多个设备取第一个
    adb_devices_output=$(adb devices)
    device_line=$(echo "$adb_devices_output" | grep -E '^[^\s]+\s+device\s*$')
    if [ -z "$device_line" ]; then
        log "没有设备连接" false
    else
        device_name=$(echo "$device_line" | awk '{print $1}')
        log "连接设备名称: $device_name" false
        # 安装
        log "adb -s $device_name install $outDir/launcher-release.apk"
        adb -s "$device_name" install "$outDir/launcher-release.apk" >>"$buildLogPath" 2>&1
        # 运行(不同app package name activity path不一样)
        appPackage="com.lyh.t1"
        # appActivityPath="$appPackage.game.UnityPlayerActivity"
        appActivityPath=com.unity3d.player.UnityPlayerActivity
        log "adb -s $device_name shell am start -n $appPackage/$appActivityPath"
        adb -s "$device_name" shell am start -n "$appPackage/$appActivityPath" >>"$buildLogPath" 2>&1
    fi
    # 结束adb服务
    adb kill-server
}

gradleHandler() {
    log "------Gradle Build------"
    log ">Gradle Clean<"
    gradle clean >>"$buildLogPath" 2>&1
    log ">Gradle Clean End<"
    # log ">Gradle Warning<"
    # gradle --warning-mode all >>"$buildLogPath" 2>&1
    # log ">Gradle Warning End<"
    log ">Build"
    gradle assembleRelease >>"$buildLogPath" 2>&1
    log ">Build End<"
    log ">Gradle Stop<"
    gradle --stop >>"$buildLogPath" 2>&1
    log ">Gradle Stop End<"
    log "------Gradle BuildEnd------"
}

buildFail() {
    log "Build Fail" false
    log "Log Info: $buildLogPath" false
    exit
}

log() {
    if [ "$2" = "false" ]; then
        echo "$1"
    else
        echo "$1" >>"$buildLogPath"
    fi
}

warningLog() {
    if [ "$2" = "false" ]; then
        echo -e "\e[33m[Warning]$1\e[0m"
    else
        log "$2"
    fi
}

errorlog() {
    if [ "$2" = "false" ]; then
        echo -e "\e[31m[Error]$1\e[0m"
    else
        log "$1"
    fi
}

hasDir() {
    if [ -d "$1" ]; then
        return 0
    else
        return 1
    fi
}

hasApk() {
    if ! hasDir "$1"; then
        return 1
    fi
    apk_files=$(find "$1" -type f -name "*.apk")
    if [ -n "$apk_files" ]; then
        return 0
    else
        return 1
    fi
}

main
