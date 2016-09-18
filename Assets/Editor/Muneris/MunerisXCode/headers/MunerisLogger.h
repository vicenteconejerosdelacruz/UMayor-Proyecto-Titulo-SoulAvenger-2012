//
//  MunerisLogger.h
//  MunerisKit
//
//  Created by Casper Lee on 26/4/12.
//  Copyright (c) 2012 Outblaze Limited. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "MunerisConstant.h"

void MLOGLEVEL(int level);
void MLOGINFO (NSString* message, ...);
void MLOGDEBUG (NSString* message, ...);
void MLOGERROR (NSString* message, ...);
void MLOGTRACE (NSString* message, ...);
void MLOGWARNING (NSString* message, ...);
void MLOGCONDITION (BOOL condition, NSString* message, ...);

#define MLOG(...) MLOGINFO(__VA_ARGS__)
#define MDEBUG(...) MLOGDEBUG(__VA_ARGS__)
#define MINFO(...) MLOGINFO(__VA_ARGS__)
#define MERROR(...) MLOGERROR(__VA_ARGS__)
#define MTRACE(...) MLOGTRACE(__VA_ARGS__)
#define MWARN(...) MLOGWARNING(__VA_ARGS__)
#define MCLOG(obj, ...) MLOGCONDITION(obj, __VA_ARGS__)
#define MERROREXCEPTION(EXCEPTION) MLOGERROR(@"\
******************************  EXCEPTION  ******************************\n\
%@\n\
%@",EXCEPTION,[EXCEPTION callStackSymbols])