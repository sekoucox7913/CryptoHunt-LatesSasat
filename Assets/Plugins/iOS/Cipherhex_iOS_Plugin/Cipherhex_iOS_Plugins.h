//
//  Cipherhex_iOS_Plugins.h
//
//  Created by Sandip on 09/04/16.
//

#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>
#import <MediaPlayer/MediaPlayer.h>
//#import <Stripe/Stripe.h>
#import <AssetsLibrary/AssetsLibrary.h>
#import <SafariServices/SFFoundation.h>
#import "UnityAppController.h"

@interface Cipherhex_iOS_Plugins : UIViewController <UIImagePickerControllerDelegate, UINavigationControllerDelegate, UIPopoverControllerDelegate,UIWebViewDelegate>
{
@public
    BOOL Download;
    int Datacount;
    CGRect PopoverRect;
    MPMoviePlayerViewController *mPlayerVC;
    UIActivityViewController *activityController;
    NSString *GalleryResponse_Object,*StripeResponse_Object,*GoogleResponse_Object;
    UIWebView *videoView1;
}
@end
