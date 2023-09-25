//
//  JFGameSDKX7MallRoleInfo.h
//  JFGAMESDK
//
//  Created by 董君龙 on 2022/9/7.
//  Copyright © 2022 绝峰. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface JFGameSDKX7MallRoleInfo : NSObject

//必填游戏角色参数，不可以设置为@""或@"0"
@property (nonatomic, copy) NSString *game_area;            //必填//角色所在的游戏区,20字符以内
@property (nonatomic, copy) NSString *game_area_id;         //必填//角色所在游戏区服ID，没有填-1
@property (nonatomic, copy) NSString *game_guid;            //必填//登录后游戏方服务端通过token解析拿到的guid
@property (nonatomic, copy) NSString *game_role_id;         //必填//游戏中角色ID信息,30字符以内
@property (nonatomic, copy) NSString *game_role_name;       //必填//游戏中角色名称,30字符以内
@property (nonatomic, copy) NSString *roleLevel;            //必填//角色等级
@property (nonatomic, copy) NSString *roleCE;               //必填//角色战力
@property (nonatomic, copy) NSString *roleStage;            //必填//角色关卡
@property (nonatomic, copy) NSString *roleRechargeAmount;   //必填//角色充值

@end

NS_ASSUME_NONNULL_END
