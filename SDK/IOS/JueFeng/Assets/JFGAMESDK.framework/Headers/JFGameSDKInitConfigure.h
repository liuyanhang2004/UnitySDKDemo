//
//  JFGameSDKInitConfigure.h
//  JFGAMESDK
//
//  Created by 董君龙 on 2021/12/16.
//  Copyright © 2021 绝峰. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface JFGameSDKInitConfigure : NSObject

@property (nonatomic, copy) NSString *appId;    //JFGameSDK提供的 appId,必填

@property (nonatomic, copy) NSString *appKey;   //JFGameSDK提供的 appKey,必填

@end

NS_ASSUME_NONNULL_END
