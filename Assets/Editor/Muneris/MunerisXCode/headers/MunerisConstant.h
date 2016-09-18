//
//  MunerisConstant.h
//  MunerisKit
//
//  Created by Jacky Yuk on 8/24/11.
//  Copyright 2011 Outblaze Limited. All rights reserved.
//

#pragma mark 2.1
typedef enum {
  MunerisLogLevelError,
  MunerisLogLevelWarning,
  MunerisLogLevelInfo,
  MunerisLogLevelDebug,
  MunerisLogLevelTrace
} MunerisLogLevel;

#pragma mark 2.0



extern NSString* const kMunerisSDKVersion;

extern NSString* const kMunerisFolder;

extern NSString* const kMunerisSDKPlaform;

extern NSString* const kMunerisAPIVersion;

extern NSString* const kMunerisSDKAgent;

extern NSString* const kMunerisStoreFolder;

extern NSString* const kMunerisStateFile;

extern NSString* const kMunerisInstallIdFile;

extern NSString* const kMunerisPlistFileFormat;

extern NSString* const kMunerisPluginsList;

extern NSString* const kMunerisInstallIdApiMethod;

extern NSString* const kMunerisPluginClassFormat;

extern NSStringEncoding const kMunerisApiEncoding;

extern NSString* const MUNERIS_ENVARS_UPDATE_SUCCESS_NOTIFICATION;

typedef enum {
  MunerisUnKnownMessage,
  MunerisTextMessage,
  MunerisCreditsMessage
//  ,
//  MunerisVersionUpdateMessage
} MunerisMessageType;


typedef enum {
  MunerisModeDebug,
  MunerisModeProduction,
  MunerisModeTesting
} MunerisMode;




typedef enum {
  MunerisApiServerError
} MunerisApiError;

typedef enum {
  MunerisBannerAdsSizes320x50,
  MunerisBannerAdsSizes768x90
} MunerisBannerAdsSizes;



