//
//  AfppUIThemeConfig.h
//  W3iPublisherSdk
//
//  Created by Bozhidar Mihaylov on 12/6/11.
//  Copyright (c) 2011 MentorMate. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol AfppUIThemeConfig <NSObject>

@optional
- (UIColor *)backgroundColor;

- (UIImage *)toolbarBackgroundImagePortrait;
- (UIImage *)toolbarBackgroundImageLandscape;

- (UIColor *)titleTextColor;

- (UIColor *)activeButtonTextColor;
- (UIColor *)inactiveButtonTextColor;

- (UIColor *)offerNameTextColor;
- (UIColor *)offerBackgroundColor;
- (UIImage *)offerBackgroundImage;

@end