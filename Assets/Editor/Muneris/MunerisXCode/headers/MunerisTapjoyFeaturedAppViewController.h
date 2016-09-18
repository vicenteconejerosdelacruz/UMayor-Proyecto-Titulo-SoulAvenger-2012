//
//  MunerisTapjoyFeaturedAppView.h
//  MunerisKit
//
//  Created by Casper Lee on 9/2/12.
//  Copyright (c) 2012 Outblaze Limited. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "TapjoyConnect.h"
#import "MunerisTakeoverDelegate.h"


@interface MunerisTapjoyFeaturedAppViewController : UIViewController {
  id<MunerisTakeoverDelegate> _takeoverDelegate;
  NSDictionary* _info;
  UIViewController* _viewController; 
}

@property (nonatomic, retain) TJCFeaturedAppModel *_featuredAppModel;

@property (nonatomic, retain) IBOutlet UIButton* _featuredAppDownloadBtn;
@property (nonatomic, retain) IBOutlet UILabel* _featuredAppName;
@property (nonatomic, retain) IBOutlet UILabel* _featuredAppPrice;
@property (nonatomic, retain) IBOutlet UIButton* _featuredAppIcon;

@property (nonatomic, retain) IBOutlet UILabel* faTitle;
@property (nonatomic, retain) IBOutlet UILabel* faTitleText;


- (IBAction)closeFeaturedApp:(id)sender;
- (IBAction) downloadFeaturedApp:(id)sender;

-(id) initWithDictionary: (NSDictionary*) dictionary withDelegate: (id<MunerisTakeoverDelegate>) delegate;
- (NSString*) decodeHtmlUnicodeCharactersToString:(NSString*)str;
- (void) showFeaturedAppWithViewController: (UIViewController*) viewController;

@end
