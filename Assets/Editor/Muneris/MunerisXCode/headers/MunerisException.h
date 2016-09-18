//
//  MunerisException.h
//  MunerisKit
//
//  Created by Jacky Yuk on 4/30/12.
//  Copyright (c) 2012 Outblaze Limited. All rights reserved.
//

#import <Foundation/Foundation.h>


@interface MunerisException : NSException
{
  NSException* _exception;
}


+(id) exceptionWithException:(NSException*) exception;

+(id) exceptionWithObject:(id) obj reason:(NSString*) reason userInfo:(NSDictionary*) userInfo;

+(void) throwWithObject:(id) obj reason:(NSString*) reason userInfo:(NSDictionary*) userInfo;

@end
