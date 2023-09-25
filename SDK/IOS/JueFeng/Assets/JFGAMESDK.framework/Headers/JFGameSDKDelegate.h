//
//  JFGameSDKDelegate.h
//  JFGAMESDK
//
//  Created by 董君龙 on 2021/12/21.
//  Copyright © 2021 绝峰. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@protocol JFGameSDKDelegate <NSObject>

/**
 *  SDK登录成功回调
 *
 *  @param success
 *    key : userId       value :    用户id
 *    key : username     value :    用户名
 *    key : token        value :    token
 */
- (void)onLoginSuccess:(NSDictionary *)success;
/**
 *  SDK登录失败回调
 *
 *  @param failure
 *    key : code          value :    错误码
 *    key : error         value :    错误原因
 */
- (void)onLoginFailure:(NSDictionary *)failure;
/**
 *  SDK初始化成功回调
 *
 *  @param success    成功
 */
- (void)onInitSuccess:(NSDictionary *)success;
/**
 *  SDK初始化失败回调
 *
 *  @param failure    失败
 */
- (void)onInitFailure:(NSDictionary *)failure;
/**
 *  SDK支付成功回调
 *
 *  @param success
 *    key : orderId       value :    订单号
 *    key : gameRole      value :    游戏角色id
 *    key : gameArea      value :    游戏区服id
 *    key : productName   value :    游戏购买物品名称
 *    key : ProductDesc   value :    游戏购买物品描述
 *    key : remark        value :    备注
 */
- (void)onPaySuccessCallback:(NSDictionary *)success;
/**
 *  SDK支付失败回调
 *
 *  @param failure
 *    key : code          value :    错误码
 *    key : error         value :    错误原因
 */
- (void)onPayFaildCallback:(NSDictionary *)failure;
/**
 *  SDK创建订单成功回调
 *
 *  @param success
 *    key : orderId       value :    订单号
 *    key : gameRole      value :    游戏角色id
 *    key : gameArea      value :    游戏区服id
 *    key : productName   value :    游戏购买物品名称
 *    key : ProductDesc   value :    游戏购买物品描述
 *    key : remark        value :    备注
 */
- (void)onCreatedOrderSuccess:(NSDictionary *)success;
/**
 *  SDK创建订单失败回调
 *  @param failure    失败
 *
 */
- (void)onCreatedOrderFailure:(NSDictionary *)failure;
/**
 *  SDK退出登录成功回调
 *
 *  @param success    成功
 */
- (void)onLogoutLoginSuccess:(NSDictionary *)success;
/**
 *  SDK退出登录失败回调
 *
 *  @param failure    失败
 */
- (void)onLogoutLoginFailure:(NSDictionary *)failure;
/**
 *  SDK切换账号成功回调
 *
 *  @param success    成功
 */
- (void)onSwitchAccountSuccess:(NSDictionary *)success;
/**
 *  SDK切换账号失败回调
 *
 *  @param failure    失败
 */
- (void)onSwitchAccountFailure:(NSDictionary *)failure;

/**
 *  SDK打开充值页面成功回调
 *
 *  @param success    成功
 */
- (void)openRechargePageSuccess:(NSDictionary *)success;
/**
 *  SDK打开充值页面失败回调
 *
 *  @param failure    失败
 */
- (void)openRechargePageFailure:(NSDictionary *)failure;

/**
 *  SDK打开小7商城页面成功回调
 *
 *  @param success    成功
 */
- (void)openWithX7MallSuccess:(NSDictionary *)success;
/**
 *  SDK打开小7商城页面失败回调
 *
 *  @param failure    失败
 */
- (void)openWithX7MallFailure:(NSDictionary *)failure;


@end

NS_ASSUME_NONNULL_END
