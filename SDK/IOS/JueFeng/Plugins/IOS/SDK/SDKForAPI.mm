//
// Unity - IOS相关接口
//

#ifdef __cplusplus
extern "C" {
#endif
    void UnitySendMessage(const char* obj, const char* method, const char* msg);
#ifdef __cplusplus
}
#endif

extern "C"{
#import <AdSupport/AdSupport.h>
#import <StoreKit/StoreKit.h>
#import "SDKForAPI.h"
#import <AudioToolbox/AudioServices.h>
#import <Classes/UnityAppController.h>
//NetWork
#import <SystemConfiguration/SCNetworkReachability.h>
#import <CoreTelephony/CTCarrier.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
// SIM
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <CoreTelephony/CTCarrier.h>

@implementation SDKForAPI

    /* 释放指针 */
    void FreePtr(void * p)
    {
        NSLog(@"FreePtr");
        free(p);
    }

    /* SDK初始化 */
    void initSDK()
    {
        NSLog(@"initSDK");
        UnitySendMessage("SDKMgr", "_initSDK", "1");
    }

    /* 获得设备信息 */
    void * getDirveInfo()
    {
        NSLog(@"getDirveInfo");
        NSString *adId = [[[ASIdentifierManager sharedManager] advertisingIdentifier] UUIDString];
        return strdup([adId UTF8String]);
    }

    /* 登陆 */
    void login()
    {
        NSLog(@"login");
		[[UnityAppController Instance] SDKLogin];
    }

    /* 退出游戏 */
    void gameExit()
    {
        NSLog(@"gameExit");
    }

    /* Facebook 分享 */
    void fbShare(int type, const char* path, const char* title)
    {
        NSLog(@"fbShare");
    }

    /* 获取SDK渠道名称 */
    void * getSdkChannel()
    {
        NSLog(@"getSdkChannel");
        return strdup("x_ios"); // 不执行strdup()方法而使用return mHost方法,导致mHost没有分配内存空间而报错.
    }

    /* 获取子渠道号 */
    void * getSubChannel()
    {
        NSLog(@"getSubChannel");
        return strdup("x_ios"); // 不执行strdup()方法而使用return mHost方法,导致mHost没有分配内存空间而报错.
    }

    /* 选择图片，剪裁选中的图片 */
    void pickPhoto()
    {
        NSLog(@"pickPhoto");
    }

    /* 拍摄图片，剪裁拍摄结果 */
    void takePhoto()
    {
        NSLog(@"takePhoto");
    }

    /* 切换帐号 */
    void logout()
    {
        NSLog(@"logout");
        [[UnityAppController Instance] SDKLogout];
    }

    /* 提交用户角色信息 */
    void extendInfoSubmit(const char* roleId, const char* roleName, const char* roleLevel, const char* roleCTimeroleCTime, const char* serverId, const char* serverName, const char* type)
    {
        NSLog(@"extendInfoSubmit");
    }

    /* 支付 */
    void pay(const char* roleId, const char* roleName, const char* roleLevel, const char* roleCTimeroleCTime, const char* serverId, const char* serverName, const char* orderNo, long money, const char* productId, const char* productName, const char* productDesc, int count, const char* productUnit, const char* extendInfo)
    {
        NSLog(@"pay");
		[[UnityAppController Instance] SDKPlay:roleId roleName:roleName roleLevel:roleLevel roleCTimeroleCTime:roleCTimeroleCTime serveId:serverId serveName:serverName orderNo:orderNo money:money productId:productId productName:productName productDesc:productDesc count:count productUnit:productUnit extendInfo:extendInfo];
    }

    /* 3D Touch */
    void TouchVibrate3D()
    {
        AudioServicesPlaySystemSound(1520);
    }

    void * cellularType() {
       char * notReachable = strdup("No Network");
       CTTelephonyNetworkInfo * info = [[CTTelephonyNetworkInfo alloc] init];
       
       NSString *currentRadioAccessTechnology;
       if (@available(iOS 12.1, *)) {
           if (info && [info respondsToSelector:@selector(serviceCurrentRadioAccessTechnology)]) {
               NSDictionary *radioDic = [info serviceCurrentRadioAccessTechnology];
               if (radioDic.allKeys.count) {
                   currentRadioAccessTechnology = [radioDic objectForKey:radioDic.allKeys[0]];
               } else {
                   return notReachable;
               }
           } else {
               
               return notReachable;
           }
       } else {
           currentRadioAccessTechnology = info.currentRadioAccessTechnology;
       }
       if (currentRadioAccessTechnology) {
           FreePtr(notReachable);
           if (@available(iOS 14.1, *)) {
               if ([currentRadioAccessTechnology isEqualToString:CTRadioAccessTechnologyNRNSA] || [currentRadioAccessTechnology isEqualToString:CTRadioAccessTechnologyNR]) {
                   return strdup("5G");
               }
           }
           if ([currentRadioAccessTechnology isEqualToString:CTRadioAccessTechnologyLTE]) {
               return strdup("4G");
           } else if ([currentRadioAccessTechnology isEqualToString:CTRadioAccessTechnologyWCDMA] || [currentRadioAccessTechnology isEqualToString:CTRadioAccessTechnologyHSDPA] || [currentRadioAccessTechnology isEqualToString:CTRadioAccessTechnologyHSUPA] || [currentRadioAccessTechnology isEqualToString:CTRadioAccessTechnologyCDMAEVDORev0] || [currentRadioAccessTechnology isEqualToString:CTRadioAccessTechnologyCDMAEVDORevA] || [currentRadioAccessTechnology isEqualToString:CTRadioAccessTechnologyCDMAEVDORevB] || [currentRadioAccessTechnology isEqualToString:CTRadioAccessTechnologyeHRPD]) {
               return strdup("3G");
           } else if ([currentRadioAccessTechnology isEqualToString:CTRadioAccessTechnologyEdge] || [currentRadioAccessTechnology isEqualToString:CTRadioAccessTechnologyGPRS] || [currentRadioAccessTechnology isEqualToString:CTRadioAccessTechnologyCDMA1x]) {
               return strdup("2G");
           } else {
               return strdup("UnknowNetwork");
           }
       } else {
           return notReachable;
       }
    }

    /* 获得当前网络状态 */
    void * getNetworkTypeFromStatus()
    {
        char * notReachable = strdup("No Network");
        //创建零地址，0.0.0.0的地址表示查询本机的网络连接状态
        struct sockaddr_storage zeroAddress;
        
        bzero(&zeroAddress, sizeof(zeroAddress));
        zeroAddress.ss_len = sizeof(zeroAddress);
        zeroAddress.ss_family = AF_INET;
        
        // Recover reachability flags
        SCNetworkReachabilityRef defaultRouteReachability = SCNetworkReachabilityCreateWithAddress(NULL, (struct sockaddr *)&zeroAddress);
        SCNetworkReachabilityFlags flags;
        
        //获得连接的标志
        BOOL didRetrieveFlags = SCNetworkReachabilityGetFlags(defaultRouteReachability, &flags);
        CFRelease(defaultRouteReachability);
        
        //如果不能获取连接标志，则不能连接网络，直接返回
        if (!didRetrieveFlags) {
            return notReachable;
        }
        
        BOOL isReachable = ((flags & kSCNetworkFlagsReachable) != 0);
        BOOL needsConnection = ((flags & kSCNetworkFlagsConnectionRequired) != 0);
        if (isReachable && !needsConnection) { }else{
            return notReachable;
        }
        if ((flags & kSCNetworkReachabilityFlagsConnectionRequired) == kSCNetworkReachabilityFlagsConnectionRequired ) {
            return notReachable;
        } else if ((flags & kSCNetworkReachabilityFlagsIsWWAN) == kSCNetworkReachabilityFlagsIsWWAN) {
            FreePtr(notReachable);
            return cellularType();
        } else {
            FreePtr(notReachable);
            return strdup("WiFi");
        }
    }
    
    bool isBlankString(NSString * r){
        if(r == nil || r == NULL)
            return true;
        if([r isKindOfClass:[NSNull class]])
            return  true;
        if ([[r stringByTrimmingCharactersInSet:[NSCharacterSet whitespaceCharacterSet]] length]==0)
            return true;
        return false;
    }

    /* 获得当前SIM卡类型 */
    void * getSIMStatus()
    {
        CTTelephonyNetworkInfo *info = [[CTTelephonyNetworkInfo alloc] init];
        CTCarrier *carrier = nil;
        if (@available(iOS 12.1, *))
        {
            NSDictionary *dic = [info serviceSubscriberCellularProviders];
            if (dic.allKeys.count)
            {
                carrier = [dic objectForKey:dic.allKeys[0]];
                if (isBlankString([carrier carrierName])&&dic.allKeys.count>=2)
                {
                    carrier = [dic objectForKey:dic.allKeys[1]];
                }
            }
        }
        else
        {
            carrier = [info subscriberCellularProvider];
        }
        
        if(isBlankString([carrier carrierName]))
        {
            // ﻿没有卡﻿返回##
            return strdup("NoSIM");
        }
        
        return strdup([[carrier carrierName] UTF8String]);
    }
	// test
    void commonMethod() {
		UnitySendMessage("SDKMgr", "_commonMethod", "");
    }
    void * methodAndReturn() {
        NSLog(@"methodAndReturn");
        return strdup("test methodAndReturn");
    }
@end
}
