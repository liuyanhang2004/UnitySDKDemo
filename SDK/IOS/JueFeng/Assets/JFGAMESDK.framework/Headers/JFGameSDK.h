//
//  JFGameSDK.h
//  JFGAMESDK
//
//  Created by 董君龙 on 2022/1/20.
//  Copyright © 2022 绝峰. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <Foundation/Foundation.h>
#import "JFGameSDKInitConfigure.h"
#import "JFGameSDKGameRoleInfo.h"
#import "JFGameSDKX7MallRoleInfo.h"
#import "JFGameSDKPayOrderInfo.h"
#import "JFGameSDKDelegate.h"

NS_ASSUME_NONNULL_BEGIN

@interface JFGameSDK : NSObject

#pragma mark 单例对象
+ (JFGameSDK *)shareInstance;

/**
 *  SDK初始化接口
 *
 *  @param configure    configure 初始化配置类
 *  @param delegate     初始化回调delegate
 */

-(void)initWithConfig:(JFGameSDKInitConfigure *)configure
             delegate:(id<JFGameSDKDelegate>)delegate;


/**
 *  SDK登录接口
 *  @param controller   当前控制器
 *  @param delegate     登录回调delegate
 *  @param isAutoLogon  是否自动登录
 */
-(void)logonWithController:(UIViewController *)controller
               isAutoLogon:(BOOL)isAutoLogon
                  delegate:(id<JFGameSDKDelegate>)delegate;


/**
 *  SDK注销登录接口
 *
 *  @param controller   当前控制器
 *  @param delegate     注销回调delegate
 */
-(void)logoutWithController:(UIViewController *)controller
                   delegate:(id<JFGameSDKDelegate>)delegate;

/**
 *  SDK同步数据接口
 *
 *  @param roleInfo         角色信息配置类
 */
-(void)updateWithRoleInfo:(JFGameSDKGameRoleInfo *)roleInfo;



/**
 *  创建订单
 *

 *  @param orderInfo     订单信息配置类
 *  @param roleInfo      角色信息配置类
 *  @param delegate      支付回调delegate
 */
-(void)payWithOrderInfo:(JFGameSDKPayOrderInfo *)orderInfo
               roleInfo:(JFGameSDKGameRoleInfo *)roleInfo
               delegate:(id<JFGameSDKDelegate>)delegate;



/**
 *  SDK账号切换接口
 *
 *  @param controller   当前控制器
 *  @param delegate     注销回调delegate
 */
-(void)switchAccountWithController:(UIViewController *)controller
                          delegate:(id<JFGameSDKDelegate>)delegate;



/**
 *  SDK活动页入口
 *  @param controller   当前控制器
 *  @param entryId      活动入口ID
 *  @param entryType    活动类型
 *  @param showScreen   1 竖屏  2 横屏
 *  @param delegate     回调delegate
 */
-(void)openActPageWithController:(UIViewController *)controller
                         entryId:(NSString *)entryId
                       entryType:(NSString *)entryType
                      showScreen:(NSString *)showScreen
                        delegate:(id<JFGameSDKDelegate>)delegate;


/**
 *  小7商城接口
 *
 *  @param X7MallRoleInfo      角色信息配置类
 *  @param delegate            回调delegate
 */
-(void)openWithX7MallRoleInfo:(JFGameSDKX7MallRoleInfo *)X7MallRoleInfo
                     delegate:(id<JFGameSDKDelegate>)delegate;




//***********************应用生命周期的回调*******************//
//在应用对应的生命周期回调中调用
/**
 @brief - (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url;
 @result 错误码
 @note 必接
 */

- (int)openURL:(NSURL *)url application:(UIApplication *)application;
- (int)openURL:(NSURL *)url sourceApplication:(NSString *)sourceApp application:(UIApplication *)application annotation:(id)annotation;
- (int)openURL:(NSURL *)url application:(UIApplication *)app options:(NSDictionary <NSString *, id>*)options;
/**
 @brief application:didRegisterForRemoteNotificationsWithDeviceToken:
 @result 错误码
 @note 必接
 */
- (int)application:(UIApplication*)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData*)deviceToken;

/**
 @brief application:didFailToRegisterForRemoteNotificationsWithError:
 @result 错误码
 @note 必接
 */
- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions;
- (int)application:(UIApplication*)application didFailToRegisterForRemoteNotificationsWithError:(NSError*)error;
- (int)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo;
- (int)applicationWillResignActive:(UIApplication *)application;
- (int)applicationDidEnterBackground:(UIApplication *)application;
- (int)applicationWillEnterForeground:(UIApplication *)application;
- (int)applicationDidBecomeActive:(UIApplication *)application;
- (int)applicationWillTerminate:(UIApplication *)application;
- (NSUInteger)application:(UIApplication *)application supportedInterfaceOrientationsForWindow:(UIWindow *)window;
- (BOOL)application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity restorationHandler:(void(^)(NSArray *  restorableObjects))restorationHandler;

@end

NS_ASSUME_NONNULL_END
