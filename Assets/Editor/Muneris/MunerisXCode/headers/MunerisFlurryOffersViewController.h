//
//  MunerisFlurryOffersViewController.h
//  MunerisKit
//
//  Created by Casper Lee on 9/2/12.
//  Copyright (c) 2012 Outblaze Limited. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "Muneris.h"
@class MunerisFlurryPlugin;

@interface MunerisFlurryOffersViewController : UIViewController <UITableViewDelegate, UITableViewDataSource>
{ 
  MunerisFlurryPlugin* _plugin;
  NSDictionary* _appCircleDict;
  NSString* _zone;
  NSMutableArray* _offersArray;
  id<MunerisTakeoverDelegate> _delegate;
  
  CGRect _rect;
  UIViewController* _viewController;
}

@property (nonatomic, retain) IBOutlet UILabel* _viewTitle;
@property (nonatomic, retain) IBOutlet UILabel* _offerMsg;

- (id) initWithPlugin:(MunerisFlurryPlugin*)plugin withDelegate:(id<MunerisTakeoverDelegate>)delegate withZone:(NSString*) zone withUserCookies:(NSDictionary*)userCookies;
- (void) showFlurryOffers: (UIViewController*) viewController;
- (IBAction)closeOffers:(id)sender;

@end
