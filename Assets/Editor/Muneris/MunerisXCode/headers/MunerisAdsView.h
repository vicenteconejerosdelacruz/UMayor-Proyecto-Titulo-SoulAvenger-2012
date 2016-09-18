//
//  MunerisAdsView.h
//  MunerisKit
//
//  Created by Casper on 14/09/2011.
//  Copyright 2011 Outblaze Limited. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "MunerisConstant.h"

@interface MunerisAdsView : UIView {
    
    id _delegate;
  
  NSString* _name;
}

@property (retain,nonatomic) id delegate;


-(id) initWithSize:(MunerisBannerAdsSizes) size;

- (void) refreshAds;

- (void) reloadAdsWithViewController: (UIViewController*) viewController;

@end
