//
//  MunerisConfiguration.h
//  MunerisKit
//
//  Created by Jacky Yuk on 5/2/12.
//  Copyright (c) 2012 Outblaze Limited. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol MunerisConfiguration <NSObject>


-(NSDictionary*) configuration;

-(NSString*) apiKey;

-(NSString*) extraApiParams;

@end
