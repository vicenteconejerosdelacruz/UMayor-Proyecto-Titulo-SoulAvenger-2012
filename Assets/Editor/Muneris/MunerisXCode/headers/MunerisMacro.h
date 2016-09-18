//
//  MunnerisMacro.h
//  MunerisKit
//
//  Created by Jacky Yuk on 8/30/11.
//  Copyright 2011 Outblaze Limited. All rights reserved.
//

#ifndef MunerisMacro_h
#define MunerisMacro_h

#import "MunerisLogger.h"

#define MUNERIS_DEPRECATED __attribute__((deprecated))

#define RANDOM() ((random() / (float)0x7fffffff ))


// HELPER MACRO
////////////////////////

#define M_MINUTE 60
#define M_HOUR   (60 * M_MINUTE)
#define M_DAY    (24 * M_HOUR)
#define M_5_DAYS (5 * M_DAY)
#define M_WEEK   (7 * M_DAY)
#define M_MONTH  (30.5 * M_DAY)
#define M_YEAR   (365 * M_DAY)

////////////////////////
// Safe releases

#define M_RELEASE_SAFELY(__POINTER) { \
if((__POINTER)){MLOGTRACE(@"Safe Release %@", __POINTER); [__POINTER release]; __POINTER = nil;}\
}

#define M_INVALIDATE_TIMER(__TIMER) {\
if((_TIMER)){\
MLOGTRACE(@"Invalidating Timer %@", __TIMER);\
[__TIMER invalidate]; __TIMER = nil; \
}\
}

#define M_RETAIN(__POINTER) {\
if(__POINTER){MLOGTRACE(@"Retain Obj %@", __POINTER);[__POINTER retain];}\
}

#define M_REPLACE_RETAINED_VALUE(xx, yy ) { \
MLOGTRACE(@"Replacing Obj %@ with:\n %@", xx , yy);\
if ((xx)) {\
[xx release];\
xx = nil;\
}\
if((yy)){ \
[yy retain];\
} \
xx = yy;\
}

// Release a CoreFoundation object safely.
#define M_RELEASE_CF_SAFELY(__REF) { \
MLOGTRACE(@"Release CoreFoundation Obj %@", __POINTER); \
if (nil != (__REF)) { CFRelease(__REF); __REF = nil;\
} }


#endif