//
//  JFGameSDKGameRoleInfo.h
//  JFGAMESDK
//
//  Created by 董君龙 on 2021/12/16.
//  Copyright © 2021 绝峰. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface JFGameSDKGameRoleInfo : NSObject

@property (nonatomic,copy) NSString *serverId;         //服务器Id，必填
@property (nonatomic,copy) NSString *serverName;       //服务器名称，必填
@property (nonatomic,copy) NSString *gameRoleName;     //角色名，必填
@property (nonatomic,copy) NSString *gameRoleID;       //角色ID，必填
@property (nonatomic,copy) NSString *gameUserBalance;  //玩家虚拟货币余额，必填，可随意
@property (nonatomic,copy) NSString *vipLevel;         //玩家vip等级，必填，可随意
@property (nonatomic,copy) NSString *gameUserLevel;    //玩家等级，必填，可随意
@property (nonatomic,copy) NSString *partyName;        //公会名称，必填，可随意
@property (nonatomic,copy) NSString *creatTime;        //角色创建时间(10位时间戳)，必填,没有传0
@property (nonatomic,copy) NSString *gameAreaId;       //游戏区id, 必填
@property (nonatomic,copy) NSString *fightPower;       //战力，必填，可随意
@property (nonatomic,copy) NSString *profession;       //角色职业, 必填，可随意
@property (nonatomic,copy) NSString *attach;           //额外信息, 必填，可随意
@property (nonatomic,copy) NSString *type;           //场景：1.创建角色 2.进入游戏 3.角色升级 4.退出游戏



@end

NS_ASSUME_NONNULL_END
