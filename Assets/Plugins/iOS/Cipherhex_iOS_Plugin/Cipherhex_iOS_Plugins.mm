//
//  Cipherhex_iOS_Plugins.m
//  Unity-iPhone
//
//  Created by Sandip on 09/04/16.
//
//

#import "Cipherhex_iOS_Plugins.h"

@implementation Cipherhex_iOS_Plugins

- (id)init
{
    Download = false;
    Datacount = 0;
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(loadStateDidChange:) name:MPMoviePlayerLoadStateDidChangeNotification object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(playbackDidFinish:) name:MPMoviePlayerDidExitFullscreenNotification object:nil];
    self = [super init];
    return self;
}

#pragma mark SharePopup_Open
-(void)iOS_ShowSharePopUp : (NSString *)urldata : (int )imgorvdo
{
    NSMutableArray *sharingItems = [NSMutableArray new];
    [sharingItems addObject:[[urldata lastPathComponent] stringByDeletingPathExtension]];
    if(imgorvdo == 0)
    {
        NSMutableURLRequest *request1 = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:urldata]];
        [NSURLConnection sendAsynchronousRequest:request1
                                           queue:[NSOperationQueue mainQueue]
                               completionHandler:^(NSURLResponse *response, NSData *data, NSError *error) {
                                   if (!error)
                                   {
                                       UIImage * img = [UIImage imageWithData:data];
                                       [sharingItems addObject:img];
                                       [self Open_Popup:sharingItems];
                                   }
                                   else
                                   {
                                       [[[UIAlertView alloc] initWithTitle:@"Error Occured !!" message:[NSString stringWithFormat:@"%@",error] delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil, nil] show];
                                   }}];
    }
    else
    {
        [sharingItems addObject:[NSURL URLWithString:urldata]];
        [self Open_Popup:sharingItems];
    }
}

-(void)Open_Popup : (NSArray *)sharingItems
{
    activityController = [[UIActivityViewController alloc] initWithActivityItems:sharingItems applicationActivities:nil
                          ];
    activityController.excludedActivityTypes = [NSArray arrayWithObjects:UIActivityTypePrint, UIActivityTypeAssignToContact, nil];
    
    [activityController setCompletionHandler:^(NSString *activityType, BOOL completed) {
        if([activityType isEqualToString: UIActivityTypeMail]){
            NSLog(@"Mail");
        }
        if([activityType isEqualToString: UIActivityTypePostToFacebook]){
            NSLog(@"Facebook");
        }
        
    }];
    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPhone) {
        [GetAppController().window.rootViewController presentViewController:activityController animated:true completion:^{
            [[UIApplication sharedApplication] setStatusBarHidden:true];
        }];
    }
    else {
        UIPopoverController *popup = [[UIPopoverController alloc] initWithContentViewController:activityController];
        [popup presentPopoverFromRect:CGRectMake(GetAppController().rootView.frame.size.width/2, GetAppController().rootView.frame.size.height, 0, 0)inView:GetAppController().rootView permittedArrowDirections:UIPopoverArrowDirectionAny animated:true];
    }
}

#pragma mark Google_Logins
-(void)iOS_SignInGoogle :(NSString* )Obj
{
    [GetAppController() iOS_SignInGooglePlus:Obj];
}

-(void) iOS_SignOutGoogle
{
    [[GIDSignIn sharedInstance]signOut];
}

#pragma mark Save Data In Gallary
-(void)iOS_SaveDatainGallary :(NSArray *)Arrydata : (int)imgorvideo
{
    if(!Download)
    {
    Download = true;
    for (int i=0;i<Arrydata.count; i++) {
      NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[Arrydata objectAtIndex:i]]];
        [NSURLConnection sendAsynchronousRequest:request
                                           queue:[NSOperationQueue mainQueue]
                               completionHandler:^(NSURLResponse *response, NSData *data, NSError *error) {
                                   NSString *sucess;
                                   if ( !error )
                                   {
                                       ALAssetsLibrary *library = [[ALAssetsLibrary alloc] init];
                                       if(imgorvideo == 1)
                                       {
                                           NSString *videoPath = [[NSSearchPathForDirectoriesInDomains(NSCachesDirectory, NSUserDomainMask, true) objectAtIndex:0] stringByAppendingPathComponent:[NSString stringWithFormat:@"%@.mp4",[[[Arrydata objectAtIndex:i] lastPathComponent] stringByDeletingPathExtension]]];
                                           [data writeToFile:videoPath atomically:true];
                                           //UISaveVideoAtPathToSavedPhotosAlbum(videoPath, nil, nil, nil);
                                           [library writeVideoAtPathToSavedPhotosAlbum:[NSURL fileURLWithPath:videoPath] completionBlock:nil];
                                           sucess = @"Video Download Successfully!";
                                           Datacount++;
                                       }
                                       else
                                       {
                                           /*  UIImageWriteToSavedPhotosAlbum([UIImage imageWithData:data], nil, nil, nil);
                                            NSString *imgPath = [[NSSearchPathForDirectoriesInDomains(NSCachesDirectory, NSUserDomainMask, YES) objectAtIndex:0] stringByAppendingPathComponent:@"Asthi.png"];
                                            [data writeToFile:imgPath atomically:YES];
                                                                                        */
                                           [library writeImageDataToSavedPhotosAlbum:data metadata:nil completionBlock:nil];
                                           sucess = @"Image Download Successfully!";
                                           Datacount++;
                                       }
                                       if (Datacount == Arrydata.count) {
                                           [[[UIAlertView alloc] initWithTitle:@"Alert" message:sucess delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil, nil] show];
                                           Download = false;
                                           Datacount = 0;
                                       }
                                   }
                                   else
                                   {
                                       [[[UIAlertView alloc] initWithTitle:@"Error Occured !!" message:[NSString stringWithFormat:@"%@",error] delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil, nil] show];
                                       Download = false;
                                       Datacount++;
                                   }}];
    }
    }
    else
    {
            [[[UIAlertView alloc] initWithTitle:@"Download is Running" message:@"Please try again after download completed.." delegate:self cancelButtonTitle:@"OK" otherButtonTitles:nil, nil] show];
    }
}

#pragma mark Stripe Payment
-(void) iOS_PaymentStripe :(NSString *)cardNo : (int)ExpMonth : (int) ExpYear : (NSString *)CVV : (NSString *)obj : (NSString *)StripeUrl
{
    /*
    [Stripe setDefaultPublishableKey:StripeUrl];
    StripeResponse_Object = obj;
    STPCard *card = [[STPCard alloc] init];
    card.number = cardNo;
    card.expMonth = ExpMonth;
    card.expYear = ExpYear;
    card.cvc = CVV;
    
    [[STPAPIClient sharedClient] createTokenWithCard:card
                                          completion:^(STPToken *token, NSError *error) {
                                              if (error)
                                              {
                                                  NSString * err = [error localizedDescription];
                                                  UnitySendMessage([StripeResponse_Object UTF8String], "StripePayError", [err UTF8String]);
                                              } else {
                                                  UnitySendMessage([StripeResponse_Object UTF8String], "StripePaySuccess",[token.tokenId UTF8String]);}   }];
     */
}

#pragma mark Thumbnail From URL Download
-(NSString *)iOS_ThumbnailfromUrl:(NSString *)urlString
{
    NSURL *Tempurl = [NSURL URLWithString:urlString];
    
    AVAsset *asset = [AVAsset assetWithURL:Tempurl];
    AVAssetImageGenerator *imageGenerator = [[AVAssetImageGenerator alloc]initWithAsset:asset];
    CMTime time1 = CMTimeMake(1, 10);
    CGImageRef imageRef = [imageGenerator copyCGImageAtTime:time1 actualTime:NULL error:NULL];
    UIImage *thumbnail = [UIImage imageWithCGImage:imageRef];
    
    NSData *data = UIImagePNGRepresentation(thumbnail);
    
    NSString *imgstring = [data base64EncodedStringWithOptions:NSDataBase64Encoding64CharacterLineLength];
    return imgstring;
}

#pragma mark PlayVideo From Buffer Url
-(void)iOS_VideoPlay :(NSString *)_videourl
{
    NSURL *streamURL = [NSURL URLWithString:_videourl];
    
    mPlayerVC = [[MPMoviePlayerViewController alloc] initWithContentURL:streamURL];
    
    mPlayerVC.moviePlayer.controlStyle = MPMovieControlStyleFullscreen;
    mPlayerVC.moviePlayer.scalingMode = MPMovieScalingModeAspectFit;
    mPlayerVC.moviePlayer.shouldAutoplay = YES;
    [mPlayerVC.moviePlayer setRepeatMode:MPMovieRepeatModeNone];
    
    [GetAppController().window.rootViewController.view addSubview:mPlayerVC.view];
    [mPlayerVC.moviePlayer play];
    
    [GetAppController().window.rootViewController presentMoviePlayerViewControllerAnimated:mPlayerVC];
    
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(iOS_VideoFinished:) name:MPMoviePlayerPlaybackDidFinishNotification object:mPlayerVC.moviePlayer];
}

- (void)iOS_VideoFinished:(NSNotification*)notification {
    [mPlayerVC.moviePlayer stop];
    [GetAppController().window.rootViewController dismissViewControllerAnimated:true completion:nil];
    [mPlayerVC.view removeFromSuperview];
    mPlayerVC = nil;
}

- (void)embedYouTube:(NSString*)url{
    url = @"https://www.youtube.com/watch?v=M6ro0mib31M";
    UIView * view = GetAppController().window.rootViewController.view;
    NSString* embedHTML = @"\
    <html><head>\
    <style type=\"text/css\">\
    body {\
    background-color: transparent;\
    color: white;\
    }\
    </style>\
    </head><body style=\"margin:0\">\
    <embed id=\"yt\" src=\"%@\" type=\"application/x-shockwave-flash\" \
    width=\"%0.0f\" height=\"%0.0f\"></embed>\
    </body></html>";
    CGRect frame1 = CGRectMake(0, 0, view.frame.size.width, view.frame.size.height);
    NSString* html = [NSString stringWithFormat:embedHTML, url, frame1.size.width, frame1.size.height];
    UIWebView *videoView = [[UIWebView alloc] initWithFrame:frame1];
    videoView.backgroundColor = [UIColor clearColor];
    videoView.opaque = NO;
    videoView.delegate = self;
    videoView1 = videoView;
    [view addSubview:videoView1];
    videoView1.mediaPlaybackRequiresUserAction = NO;
    [videoView1 loadHTMLString:html baseURL:nil];
}

- (void)loadStateDidChange:(NSNotification*)notification
{
    NSLog(@"________loadStateDidChange");
}

- (void)playbackDidFinish:(NSNotification*)notification
{
    NSLog(@"________DidExitFullscreenNotification");
}

#pragma mark Camera And Gallary open/select
- (void)iOS_ShowPhotoPicker:(UIImagePickerControllerSourceType)type maxWidth:(int)maxWidth maxHeight:(int)maxHeight : (NSString *)obj
{
    GalleryResponse_Object = obj;
#if !UNITY_5_0_0
    UIImagePickerController *picker = [[[UIImagePickerController alloc] init] autorelease];
#else
    UIImagePickerController *picker = [[UIImagePickerController alloc] init];
#endif
    picker.delegate = self;
    picker.sourceType = type;
    picker.allowsEditing = false;
    [NSObject cancelPreviousPerformRequestsWithTarget:self];
    UIViewController *vc = GetAppController().window.rootViewController;
    picker.popoverPresentationController.sourceView = vc.view;
    picker.popoverPresentationController.sourceRect = PopoverRect;
    [vc presentViewController:picker animated:YES completion:nil];
    
}

- (void)imagePickerControllerDidCancel:(UIImagePickerController*)picker
{
    [GetAppController().window.rootViewController dismissViewControllerAnimated:true completion:nil];
}

- (void)imagePickerController:(UIImagePickerController*)picker didFinishPickingMediaWithInfo:(NSDictionary*)info
{
    // grab the image
    UIImage *image = [info objectForKey: UIImagePickerControllerOriginalImage];
    
    [GetAppController().window.rootViewController dismissViewControllerAnimated:true completion:nil];
    image = [self compressImage:image];
    NSData *data = UIImagePNGRepresentation(image);
    NSString *string = [data base64EncodedStringWithOptions:NSDataBase64Encoding64CharacterLineLength];
    
    UnitySendMessage([GalleryResponse_Object UTF8String], "OnPhotoPick",[string UTF8String]);
    
}

- (UIImage *)compressImage:(UIImage *)image {
    float actualHeight = image.size.height;
    float actualWidth = image.size.width;
    float maxHeight = 800.0; //new max. height for image
    float maxWidth = 600.0; //new max. width for image
    float imgRatio = actualWidth/actualHeight;
    float maxRatio = maxWidth/maxHeight;
    float compressionQuality = 0.8;
    
    if (actualHeight > maxHeight || actualWidth > maxWidth){
        if(imgRatio < maxRatio){
            //adjust width according to maxHeight
            imgRatio = maxHeight / actualHeight;
            actualWidth = imgRatio * actualWidth;
            actualHeight = maxHeight;
        }
        else if(imgRatio > maxRatio){
            //adjust height according to maxWidth
            imgRatio = maxWidth / actualWidth;
            actualHeight = imgRatio * actualHeight;
            actualWidth = maxWidth;
        }
        else{
            actualHeight = maxHeight;
            actualWidth = maxWidth;
        }
    }
    
    CGRect rect = CGRectMake(0.0, 0.0, actualWidth, actualHeight);
    UIGraphicsBeginImageContext(rect.size);
    [image drawInRect:rect];
    UIImage *img = UIGraphicsGetImageFromCurrentImageContext();
    NSData *imageData = UIImageJPEGRepresentation(img, compressionQuality);
    UIGraphicsEndImageContext();
    
    return [UIImage imageWithData:imageData];
}

- (void)dealloc
{
#if !UNITY_5_0_0
    [super dealloc];
#endif
}

@end

static Cipherhex_iOS_Plugins* native = nil;
extern "C"
{
    void InitStream()
    {
        if (native == nil) native = [[Cipherhex_iOS_Plugins alloc] init];
    }
    
    void PlayVideoWithBufferurl(char* url)
    {
        InitStream();
        [native iOS_VideoPlay:[NSString stringWithUTF8String:url]];
    }
    
    //Google Sign In Out !!!!!
    void OnSignInGoogle (char *Objectname_forResponse)
    {
        InitStream();
//      [native embedYouTube:[NSString stringWithUTF8String:Objectname_forResponse]];
        [native iOS_SignInGoogle:[NSString stringWithUTF8String:Objectname_forResponse]];
    }
    void OnSignOutGoogle ()
    {
        InitStream();
        [native iOS_SignOutGoogle];
    }
    
    void PaymentUsingStripe(char *Objectname_forResponse,char* cardNo,int ExpMonth ,int ExpYear,char* CVV,char*StripeUrl)
    {
        InitStream();
        [native iOS_PaymentStripe:[NSString stringWithUTF8String:cardNo] :ExpMonth :ExpYear :[NSString stringWithUTF8String:CVV] :[NSString stringWithUTF8String:Objectname_forResponse]:[NSString stringWithUTF8String:StripeUrl]];
    }
    
    //For Camera And gallery Open
    void LoadImagePicker(char *Objectname_forResponse,int maxWidth, int maxHeight)
    {
        InitStream();
        native->PopoverRect = CGRectMake(0, 0, maxWidth, maxHeight);
        [native iOS_ShowPhotoPicker:UIImagePickerControllerSourceTypePhotoLibrary maxWidth:maxWidth maxHeight:maxHeight :[NSString stringWithUTF8String:Objectname_forResponse]];
    }
    
    void LoadCameraPicker(char *Objectname_forResponse,int maxWidth, int maxHeight)
    {
        InitStream();
        native->PopoverRect = CGRectMake(0, 0, maxWidth, maxHeight);
        [native iOS_ShowPhotoPicker:UIImagePickerControllerSourceTypeCamera maxWidth:maxWidth maxHeight:maxHeight :[NSString stringWithUTF8String:Objectname_forResponse]];
    }
    
    //imgorvideo == 0 image else Video
    void SaveDataInDevice(char *data,int imgorvideo)
    {
        InitStream();
        NSData *data1 = [[NSString stringWithUTF8String:data] dataUsingEncoding:NSUTF8StringEncoding];
        NSArray *values = [NSJSONSerialization JSONObjectWithData:data1 options:NSJSONReadingMutableContainers error:nil];
        [native iOS_SaveDatainGallary: values : imgorvideo];
    }
    
    //imgorvideo == 0 image else Video
    void ShowSharingOptions(char *data,int imgorvideo)
    {
        InitStream();
        [native iOS_ShowSharePopUp:[NSString stringWithUTF8String:data]:imgorvideo];
    }
    
    const char *GetThumbnailFromUrl(char *url ,int interval){
        InitStream();
        return [native iOS_ThumbnailfromUrl:[NSString stringWithUTF8String:url]].UTF8String;
    }
}