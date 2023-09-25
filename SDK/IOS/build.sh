#! /bin/bash
: <<!
1、Archive
xcodebuild archive 
            -archivePath <archivePath>
            -project <projectName>
            -workspace <workspaceName>
            -scheme <schemeName>    #从-list命令中获取
            -configuration <Debug|Release>

2、Export
xcodebuild -exportArchive
            -archivePath <xcarchivepath>
            -exportPath <destinationpath>
            -exportOptionsPlist <plistpath>   #这个plist文件可以通过打一次ipa包里面去获取

3、Clean
xcodebuild clean
            -workspace <workspaceName>
            -scheme <schemeName>    #从-list命令中获取
            -configuration <Debug|Release>
参考：https://juejin.cn/user/52367544549774/posts
!
: <<!
构建IOS签名的时候提示errSecInternalComponent错误
出现这个问题的主要原因是，以ssh方式到slave机上，默认是没有账户的，但是访问钥匙串要求必须有用户身份，
解决办法
添加一步输入密码解锁钥匙串，给一个用户身份。 build步骤前添加一步解锁钥匙串。
security unlock-keychain -p '123456' ~/Library/Keychains/login.keychain
!

LANG=C.UTF-8
# IOS项目根目录
buildDir=$1
# IPA保存目录
outDir=$2
# log path
buildLogPath="$outDir/BuildLog.txt"
date=$(date "+%Y-%m-%d %H:%M:%S")
# 通过ssh连接打包机<需保证能通过密钥无需输入密码正确登录>
host=blackwings@192.168.1.125
privateKeyPath=./id_rsa
# 本机压缩工具
zipTool="D:/Program Files/7-Zip/7z"

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
    if ! ssh -q -i $privateKeyPath $host exit; then
        log "打包机连接失败 $host"
        buildFail
    fi
    {
        # 创建临时文件夹
        execute "rm -rf ~/Temp"
        execute "mkdir ~/Temp"

        # 压缩工程
        "$zipTool" a "IOSProject.zip" "../../Build/IOS/IOSProject"

        # 拷贝工程压缩包
        scp -i ./id_rsa -r IOSProject.zip $host:~/Temp

        # 拷贝ExportOptions.plist
        scp -i ./id_rsa -r ExportOptions.plist $host:~/Temp

        rm -f IOSProject.zip

        # 解压后设置权限
        execute "
                cd ~/Temp
                unzip IOSProject.zip >> /dev/null
                chmod -R 777 IOSProject
                rm IOSProject.zip
                "

        # 编译并导出xcarchive
        log "编译并导出xcarchive"
        execute "
            cd ~/Temp/IOSProject
            security unlock-keychain -p '123456' ~/Library/Keychains/login.keychain
            xcodebuild archive -project Unity-iPhone.xcodeproj -scheme Unity-iPhone -archivePath '../IOSProject.xcarchive' -destination 'generic/platform=IOS'
            xcodebuild clean >> /dev/null 2>&1
            "
    } >>"$buildLogPath" 2>&1
    buildSuccess=$(execute "
            cd ~/Temp
            [ -e *.xcarchive ] && echo 1 || echo 0
        ")
    if [ "$buildSuccess" = "0" ]; then
        buildFail
    fi

    # 导出ipa
    # 没有开通开发者账号无法在Xcode中构建ipa，但是可以使用命令行构建(前提是使用Xcode自动签名运行过项目本地会存有证书和签名文件)ios开发者享有的服务也是无法使用
    # ExportOptions.plist文件生成
    # https://juejin.cn/post/6997226858022469639
    log "导出ipa"
    execute "cd ~/Temp/IOSProject
        security unlock-keychain -p '123456' ~/Library/Keychains/login.keychain
        xcodebuild -exportArchive -archivePath '../IOSProject.xcarchive' -exportPath '../Development' -configuration Release -exportOptionsPlist ../ExportOptions.plist
        " >>"$buildLogPath" 2>&1
    # [Obsolete] 适用于没有开发者账号使用自动签名的情况将生成的.xcarchive文件手动打包ipa只能用于简单测试无法使用ios的服务
    # executeScript ./xcarchive2ipa.sh \~/Temp/IOSProject.xcarchive \~/Temp/Development >/dev/null
    buildSuccess=$(execute "
                if [ ! -d  ~/Temp/Development ]; then
                    echo 0
                    exit
                fi
                cd ~/Temp/Development
                [ -e *.ipa ] && echo 1 || echo 0
        ")
    if [ "$buildSuccess" = "0" ]; then
        buildFail
    fi

    # 拷贝ipa到目标机器上
    scp -i ./id_rsa -r $host:~/Temp/Development/*.ipa $outDir >>/dev/null

    # 清理临时文件夹
    # execute "rm -rf ~/Temp"
    # todo 清理以前的所有文件
    # execute "
    #         rm -rf /Users/blackwings/Library/Developer/CoreSimulator/*
    #         rm -rf /Users/blackwings/Library/Developer/Xcode/Archives/*
    #         rm -rf /Users/blackwings/Library/Developer/Xcode/DerivedData/*
    #         rm -rf /Users/blackwings/Library/Developer/Xcode/iOS DeviceSupport/*
    #         "

    log "Build Success" false
    log "OutDir: $outDir" false
}

execute() {
    ssh -i $privateKeyPath $host /bin/bash <<EOT
	${1}
EOT
}

executeScript() {
    r=$(cat "$1")
    # 参数处理
    args=("$@")
    for ((i = 1; i < ${#args[@]}; i += 1)); do
        p="${args[i]}"
        r=${r/"\$${i}"/$p}
    done
    execute "$r"
}

hasDir() {
    if [ -d "$1" ]; then
        return 0
    else
        return 1
    fi
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

main
