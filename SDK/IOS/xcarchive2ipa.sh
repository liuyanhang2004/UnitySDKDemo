#!/bin/bash
: <<!
由于没有开发者账号使用自动签名不能使用内置的打包功能
将生成的.xcarchive归档文件手动打包ipa
只能用于简单测试无法使用ios的服务
如Notifications编译时可能会报错your development team does not support the Push Notifications capability
!
xcarchivePath=$1
outPath=$2

if [ -z "$xcarchivePath" ] || [ -z "$outPath" ]; then
    echo "参数不完整"
    exit
fi
if ! [ -d "$xcarchivePath" ]; then
    echo "$xcarchivePath不存在"
    exit
fi

mkdir -p "$outPath"
cd "$outPath" || exit
rm -rf Payload
rm -f app.ipa
mkdir Payload
cp -r "$xcarchivePath/Products/Applications/." "$outPath/Payload"
# windows
# zip="~\Program Files\7-Zip\7z"
# "zip" a -r "temp.zip" "Payload" >> /dev/null
zip -r "temp.zip" "Payload" >> /dev/null
mv "temp.zip" "app.ipa"
rm -rf Payload
