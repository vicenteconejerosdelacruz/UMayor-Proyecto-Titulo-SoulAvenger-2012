//
//  FlurryVideoOffer.h
//  Flurry iPhone Analytics Agent
//
//  Copyright 2010 Flurry, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface FlurryVideoOffer : NSObject {
    
	NSString *videoName;
	UIImage *videoThumbnail;
    NSString *description;
    NSString *ratingCode;
	NSString *videoReleaseDate;
	NSNumber *videoLength;
    
    NSString *videoUrl;
	NSString *actionUrl;
}

@property (retain) NSString *videoName;
@property (retain) UIImage *videoThumbnail;
@property (retain) NSString *description;
@property (retain) NSString *ratingCode;
@property (retain) NSString *videoReleaseDate;
@property (retain) NSNumber *videoLength;

@property (retain) NSString *videoUrl;
@property (retain) NSString *actionUrl;

@end
