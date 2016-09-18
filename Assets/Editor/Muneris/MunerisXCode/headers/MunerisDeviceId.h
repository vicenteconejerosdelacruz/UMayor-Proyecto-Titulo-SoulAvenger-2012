//
//  MunerisDeviceId.h
//  MunerisKit
//
//  Created by Casper Lee on 13/5/12.
//  Copyright (c) 2012 Outblaze Limited. All rights reserved.
//

#import <Foundation/Foundation.h>
@class MunerisStore;

@interface MunerisDeviceId : NSObject {

  MunerisStore* _store;
  
  NSDictionary* _dictionary;
  
  NSString* _installId;
  
  NSString* _macAddress;
}

-(id) initWithStore:(MunerisStore*)store;

-(NSDictionary*) getDictionary;

-(NSString*) getInstallId;

-(NSString*) getIDFA;

-(NSString*) getIDFV;

-(NSString*) getUDID;

-(NSString*) getOdin1;

-(NSString*) getOpenUDID;

-(NSString*) getMacAddress;

@end
