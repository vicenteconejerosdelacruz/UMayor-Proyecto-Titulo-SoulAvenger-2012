package muneris.android.unity;

import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;


import muneris.android.iap.Iap;
import muneris.android.iap.PurchaseCallback;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import muneris.android.core.Muneris;
import muneris.android.core.banner.BannerSize;
import muneris.android.core.exception.MunerisException;
import muneris.android.core.messages.CreditsMessage;
import muneris.android.core.messages.Message;
import muneris.android.core.messages.MessageType;
import muneris.android.core.messages.TextMessage;
import muneris.android.core.plugin.Listeners.AdListener;

import muneris.android.core.plugin.Listeners.MessagesListener;
import muneris.android.core.plugin.Listeners.OffersListener;
import muneris.android.core.plugin.Listeners.TakeoverListener;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.DialogInterface.OnClickListener;
import android.util.Log;
import android.view.View;
import android.view.ViewManager;
import android.view.ViewGroup.LayoutParams;
import android.widget.RelativeLayout;
import android.widget.LinearLayout;

import com.unity3d.player.UnityPlayer;

public class Bridge
{

    private static final String MANAGER_OBJECT_NAME = "Muneris";

    private static volatile RelativeLayout adView;

    private static volatile boolean adsClosed;

    private static volatile AlertDialog currentAlert;

    public volatile static boolean isShowingTakeover = false;

    public static void boot( Activity activity )
    {
        Muneris.addCallback( new IapDelegate() );
    }

    private static class MessagesDelegate implements MessagesListener
    {

        @Override
        public void onMessagesFailed( MunerisException arg0 )
        {
            UnityPlayer.UnitySendMessage( MANAGER_OBJECT_NAME, "onMessagesFailed", arg0.getLocalizedMessage() );
        }

        @Override
        public void onMessagesReceived( List<Message> arg0 )
        {
            UnityPlayer.UnitySendMessage( MANAGER_OBJECT_NAME, "onMessagesReceived", serializeMessages( arg0 ) );
        }
    }

    private static class TakeoverDelegate implements TakeoverListener
    {

        @Override
        public void didFailedToLoadTakeover()
        {
            isShowingTakeover = false;
            UnityPlayer.UnitySendMessage( MANAGER_OBJECT_NAME, "didFailedToLoadTakeover", "" );
        }

        @Override
        public void didFinishedLoadingTakeover()
        {
            Log.i( "Bridge", "didFinishedLoadingTakeover" );
            UnityPlayer.UnitySendMessage( MANAGER_OBJECT_NAME, "didFinishedLoadingTakeover", "" );
        }

        @Override
        public void onDismissTakeover()
        {
            isShowingTakeover = false;
            UnityPlayer.UnitySendMessage( MANAGER_OBJECT_NAME, "onDismissTakeover", "" );
        }

        @Override
        public boolean shouldShowTakeover()
        {
            Log.i( "Bridge", "Showing takeover..." );
            isShowingTakeover = true;
            return true;
        }
    }

    private static class OffersDelegate implements TakeoverListener, OffersListener
    {

        @Override
        public void onMessagesFailed( MunerisException arg0 )
        {
            //Log.i("MunerisBridge", "OffersDelegate.onMessagesFailed Called!");
            UnityPlayer.UnitySendMessage( MANAGER_OBJECT_NAME, "onOfferMessagesFailed", arg0.getLocalizedMessage() );
        }

        @Override
        public void onMessagesReceived( List<Message> arg0 )
        {
            //Log.i("MunerisBridge", "OffersDelegate.onMessagesReceived Called!");
            UnityPlayer.UnitySendMessage( MANAGER_OBJECT_NAME, "onOfferMessagesReceived", serializeMessages( arg0 ) );
        }

        @Override
        public void didFailedToLoadTakeover()
        {
            isShowingTakeover = false;
            UnityPlayer.UnitySendMessage( MANAGER_OBJECT_NAME, "onOfferFailed", "" );
        }

        @Override
        public void didFinishedLoadingTakeover()
        {
            Log.i( "Bridge", "didFinishedLoadingTakeover" );
            UnityPlayer.UnitySendMessage( MANAGER_OBJECT_NAME, "onOfferClosed", "" );
        }

        @Override
        public void onDismissTakeover()
        {
            isShowingTakeover = false;
            UnityPlayer.UnitySendMessage( MANAGER_OBJECT_NAME, "onOfferClosed", "" );
        }

        @Override
        public boolean shouldShowTakeover()
        {
            Log.i( "Bridge", "Showing takeover" );
            isShowingTakeover = true;
            return true;
        }
    }

    private static String serializeMessages( List<Message> messages )
    {
        final JSONArray res = new JSONArray();

        for ( Message msg : messages )
        {
            final JSONObject obj = new JSONObject();

            try
            {
                obj.put( "type", msg.getType().toString() );
                obj.put( "subj", msg.getSubject() );
                obj.put( "body", ( msg instanceof TextMessage ) ? ( (TextMessage) msg ).getText() : "" );
                obj.put( "credits", ( msg instanceof CreditsMessage ) ? ( (CreditsMessage) msg ).getCredits() : 0 );
            }
            catch ( JSONException e )
            {
                // TODO Auto-generated catch block
                e.printStackTrace();
                break;
            }

            res.put( obj );
        }

        return res.toString();
    }

    public static void displayNativeAlert( String title, String message, String options )
    {
        final AlertDialog.Builder builder = new AlertDialog.Builder( UnityPlayer.currentActivity );
        builder.setTitle( title );
        builder.setMessage( message );
        builder.setCancelable( false );

        final JSONArray parts;

        try
        {
            parts = new JSONArray( options );
        }
        catch ( Exception err )
        {
            err.printStackTrace();
            return;
        }

        class AlertListener implements OnClickListener
        {

            final String option;

            public AlertListener( String option )
            {
                this.option = option;
            }

            @Override
            public void onClick( DialogInterface arg0, int arg )
            {
                UnityPlayer.UnitySendMessage( MANAGER_OBJECT_NAME, "onAlertClosed", option );
            }
        }

        if ( parts.length() > 0 )
            builder.setPositiveButton( parts.optString( 0 ), new AlertListener( parts.optString( 0 ) ) );
        if ( parts.length() > 1 )
            builder.setNeutralButton( parts.optString( 1 ), new AlertListener( parts.optString( 1 ) ) );
        if ( parts.length() > 2 )
            builder.setNegativeButton( parts.optString( 2 ), new AlertListener( parts.optString( 2 ) ) );

        class Runner implements Runnable
        {

            public void run()
            {
                currentAlert = builder.create();
                currentAlert.show();
            }
        }

        UnityPlayer.currentActivity.runOnUiThread( new Runner() );
    }

    public static void dismissCurrentAlert()
    {
        UnityPlayer.currentActivity.runOnUiThread( new Runnable()
        {
            public void run()
            {
                if ( currentAlert != null )
                {
                    currentAlert.dismiss();
                    currentAlert = null;
                }
            }
        } );
    }

    public static class AdDelegate implements AdListener
    {

        final int alignment;

        public AdDelegate( int alignment )
        {
            this.alignment = alignment;
        }

        @Override
        public void onBannerClosed( final View arg0 )
        {
            UnityPlayer.currentActivity.runOnUiThread( new Runnable()
            {
                public void run()
                {
                    UnityPlayer.UnitySendMessage( MANAGER_OBJECT_NAME, "onBannerClosed", "" );
                }
            } );
        }

        @Override
        public void onBannerFailed( final String arg0 )
        {
            UnityPlayer.currentActivity.runOnUiThread( new Runnable()
            {
                public void run()
                {
                    UnityPlayer.UnitySendMessage( MANAGER_OBJECT_NAME, "onBannerFailed", ( arg0 != null ) ? arg0.toString() : "" );
                }
            } );
        }

        @Override
        public void onBannerLoaded( final View ad )
        {
            UnityPlayer.currentActivity.runOnUiThread( new Runnable()
            {
                public void run()
                {
                    if ( adsClosed )
                        return;

                    if ( adView != null )
                    {
                        final ViewManager parent = (ViewManager) adView.getParent();
                        if ( parent != null )
                            parent.removeView( adView );
                    }

                    adView = new RelativeLayout( UnityPlayer.currentActivity );
                    adView.setClickable( false );

                    UnityPlayer.currentActivity.addContentView( adView, new LayoutParams( LayoutParams.FILL_PARENT, LayoutParams.FILL_PARENT ) );
                    adView.removeAllViews();

                    final RelativeLayout.LayoutParams adparams = new RelativeLayout.LayoutParams( LayoutParams.WRAP_CONTENT, LayoutParams.WRAP_CONTENT );
                    if ( alignment == 1 )
                        adparams.addRule( RelativeLayout.ALIGN_PARENT_BOTTOM );

                    adparams.addRule( RelativeLayout.CENTER_HORIZONTAL );

                    final ViewManager p = (ViewManager) ad.getParent();
                    if ( p != null )
                        p.removeView( ad );

                    adView.addView( ad, adparams );

                    UnityPlayer.UnitySendMessage( MANAGER_OBJECT_NAME, "onBannerLoaded", ad.toString() );
                }
            } );
        }
    }

    public static class IapDelegate implements PurchaseCallback
    {

        @Override
        public void onPurchaseComplete( final String s )
        {
            UnityPlayer.currentActivity.runOnUiThread( new Runnable()
            {
                @Override
                public void run()
                {
                    Log.i( "MunerisUnityBridge", "IapDelegate.IapSuccess: " + s );
                    UnityPlayer.UnitySendMessage( MANAGER_OBJECT_NAME, "IapSuccess", s );
                }
            } );
        }

        @Override
        public void onPurchaseFail( final String s, MunerisException e )
        {
            UnityPlayer.currentActivity.runOnUiThread( new Runnable()
            {
                @Override
                public void run()
                {
                    Log.i( "MunerisUnityBridge", "IapDelegate.IapFailed: " + s );
                    UnityPlayer.UnitySendMessage( MANAGER_OBJECT_NAME, "IapFailed", s );
                }
            } );
        }

        @Override
        public void onPurchaseCancel( final String s )
        {
            UnityPlayer.currentActivity.runOnUiThread( new Runnable()
            {
                @Override
                public void run()
                {
                    Log.i( "MunerisUnityBridge", "IapDelegate.Cancel: " + s );
                    UnityPlayer.UnitySendMessage( MANAGER_OBJECT_NAME, "IapCancelled", s );
                }
            } );
        }
    }


    /**
     * Interface for displaying ads
     */
    public static void loadAds( final String size, final String zone, final int alignment )
    {
        UnityPlayer.currentActivity.runOnUiThread( new Runnable()
        {
            @Override
            public void run()
            {
                adsClosed = false;
                Muneris.loadAd( Enum.valueOf( BannerSize.class, size ), zone, new AdDelegate( alignment ), UnityPlayer.currentActivity );
            }
        } );
    }

    /**
     * Interface for displaying ads with default size
     */
    public static void loadAds( final String zone, final int alignment )
    {
        UnityPlayer.currentActivity.runOnUiThread( new Runnable()
        {
            @Override
            public void run()
            {
                adsClosed = false;
                Muneris.loadAd( zone, new AdDelegate( alignment ), UnityPlayer.currentActivity );
            }
        } );
    }
        
    /**
     * Interface for closing displayed ads
     */
    public static void closeAds()
    {
        UnityPlayer.currentActivity.runOnUiThread( new Runnable()
        {
            @Override
            public void run()
            {
                adsClosed = true;

                if ( adView != null )
                {
                    ViewManager view = (ViewManager) adView.getParent();
                    if ( view != null )
                        view.removeView( adView );

                    adView = null;
                }
            }
        } );
    }

    /**
     * Interface for displaying takeovers
     */
    public static void loadTakeover( final String zone )
    {
        UnityPlayer.currentActivity.runOnUiThread( new Runnable()
        {
            @Override
            public void run()
            {
                Muneris.loadTakeover( zone, new TakeoverDelegate(), UnityPlayer.currentActivity );
            }
        } );
    }

    /**
     * Interface for displaying offers
     */
    public static void showOffers()
    {
        UnityPlayer.currentActivity.runOnUiThread( new Runnable()
        {
            @Override
            public void run()
            {
                Muneris.showOffers( new OffersDelegate(), UnityPlayer.currentActivity );
            }
        } );
    }

    /**
     * Interface for displaying customer support
     */
    public static void showCustomerSupport()
    {
        UnityPlayer.currentActivity.runOnUiThread( new Runnable()
        {
            @Override
            public void run()
            {
                Muneris.showCustomerSupport( UnityPlayer.currentActivity );
            }
        } );
    }

    /**
     * Interface for displaying more apps
     */
    public static void showMoreApps()
    {
        UnityPlayer.currentActivity.runOnUiThread( new Runnable()
        {
            @Override
            public void run()
            {
                Muneris.showMoreApps( UnityPlayer.currentActivity );
            }
        } );
    }

    /**
     * Interface for checking for a version update
     */
    public static void checkVersionUpdate()
    {
        UnityPlayer.currentActivity.runOnUiThread( new Runnable()
        {
            @Override
            public void run()
            {
                Muneris.checkVersionUpdate( UnityPlayer.currentActivity );
            }
        } );
    }

    /**
     * Interface for performing in-app purchases
     */
    public static void requestPurchase( final String name )
    {
        UnityPlayer.currentActivity.runOnUiThread( new Runnable()
        {
            @Override
            public void run()
            {
                Log.i( "MunerisUnityBridge", "Requesting purchase for item: " + name );
                Iap.requestPurchase( name, UnityPlayer.currentActivity );
            }
        } );
    }

    /**
     * Interface for PPA
     */
    public static void completeAction( final String name )
    {
        UnityPlayer.currentActivity.runOnUiThread( new Runnable()
        {
            @Override
            public void run()
            {
                Muneris.actionComplete( name );
            }
        } );
    }

    /**
     * Interface for checking messages
     */
    public static void checkMessages( final String types )
    {
        UnityPlayer.currentActivity.runOnUiThread( new Runnable()
        {
            @Override
            public void run()
            {
                final String[] parts = types.split( "," );
                MessageType[] types = new MessageType[parts.length];

                int x;
                for ( x = 0; x < parts.length; x++ )
                {
                    if ( parts[x].equals( "c" ) )
                        types[x] = MessageType.Credits;
                    else if ( parts[x].equals( "t" ) )
                        types[x] = MessageType.Text;
                    else
                        types[x] = MessageType.UnKnown;
                }

                Muneris.checkMessages( new MessagesDelegate(), types );
            }
        } );
    }

    /**
     * Interface for logging analytics events
     */
    public static void logEvent( final String name, final String parameters )
    {
        UnityPlayer.currentActivity.runOnUiThread( new Runnable()
        {
            @Override
            public void run()
            {
                final Map<String, String> res = new HashMap<String, String>();

                try
                {
                    final JSONObject parms = new JSONObject( parameters );

                    @SuppressWarnings("rawtypes")
                    final Iterator it = parms.keys();
                    while ( it.hasNext() )
                    {
                        final String k = (String) it.next();
                        final Object v = parms.opt( k );

                        res.put( k, ( v != null ) ? v.toString() : "null" );
                    }
                }
                catch ( Exception e )
                {
                    e.printStackTrace();
                    return;
                }

                Log.i( "MunerisBridge", "Logging event: " + name + " params: " + res );
                Muneris.logEvent( name, res );
            }
        } );
    }

    /**
     * Interface for checking if offers are available
     */
    public static boolean hasOffers()
    {
        return Muneris.hasOffers();
    }

    /**
     * Interface for checking if more apps is available
     */
    public static boolean hasMoreApps()
    {
        return Muneris.hasMoreApps();
    }
}
