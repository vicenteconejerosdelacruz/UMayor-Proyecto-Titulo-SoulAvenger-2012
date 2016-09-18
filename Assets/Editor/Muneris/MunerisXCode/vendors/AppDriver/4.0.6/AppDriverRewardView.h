//
//  AppDriverRewardView.h
//  AppDriverLibraryManagement
//
//  Created by qu.xiaoyi on 11/10/05.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "AppDriverRequest.h"

@interface AppDriverRewardView : UIView <UIWebViewDelegate,UIAlertViewDelegate>{
    UIWebView *_webView;
    UIView *_activatorBackground;
	UIActivityIndicatorView *_activityIndicator;
	UILabel *_loadingLbl;
    bool showOutOfApp;
}

@property (nonatomic, retain) UIWebView *webView;

- (void)loadRequest:(AppDriverRequest *)request;
- (void)presentInSuperView:(UIView *)superView Animation:(BOOL)withAnimation;
- (void)showLoadErrorAlert;
@end
