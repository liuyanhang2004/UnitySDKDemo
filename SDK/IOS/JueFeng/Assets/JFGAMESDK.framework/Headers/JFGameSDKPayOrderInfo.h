//
//  JFGameSDKPayOrderInfo.h
//  JFGAMESDK
//
//  Created by 董君龙 on 2021/12/16.
//  Copyright © 2021 绝峰. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface JFGameSDKPayOrderInfo : NSObject

@property(nonatomic,copy) NSString *goodsID;      //商品ID IAP时为苹果开发者后台配置的商品id，必填
@property(nonatomic,copy) NSString *productName;  //商品名称，必填
@property(nonatomic,copy) NSString *cpOrderID;    //游戏订单ID，必填
@property(nonatomic,assign) NSUInteger count;     //商品数量，必填
@property(nonatomic,copy) NSString *amount;         //商品总价,必填，这个很重要
@property(nonatomic,copy) NSString *productDesc;  //商品描述，必填，如果数量为1，使用商品名称
@property(nonatomic,copy) NSString *callbackUrl;  //购买回调地址，必填,优先使用服务器端配置的
@property(nonatomic,copy) NSString *extrasParams; //透传字段，必填，服务器回调原样传递
@property(nonatomic,assign) float price;          //商品单价，必填,如果渠道需要，Quick将通过总价和数量来计算
@property(nonatomic,copy) NSString *quantifier;   //商品量词，必填，可以为@""空串
@property (nonatomic,copy) NSString *remarkInfo;   //备注, 必填,可备注cpOrderID


@end

NS_ASSUME_NONNULL_END
