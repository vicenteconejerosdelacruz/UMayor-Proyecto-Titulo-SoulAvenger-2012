//
//  MunerisInboxPlugin.h
//  MunerisKit
//
//  Created by Jacky Yuk on 9/7/11.
//  Copyright 2011 Outblaze Limited. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "AbstractMunerisPlugin.h"
#import "MunerisApiDelegate.h"
#import "MunerisInboxDelegate.h"
#import "MunerisInboxPluginProtocol.h"

@interface MunerisInboxPlugin : AbstractMunerisPlugin <MunerisApiDelegate, MunerisInboxPluginProtocol>
{ 
  id<MunerisInboxDelegate> _delegate;
}

@end