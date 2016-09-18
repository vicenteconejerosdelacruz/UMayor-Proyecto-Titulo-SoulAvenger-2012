package muneris.android.unity;

import java.lang.reflect.Method;
import muneris.android.core.Muneris;

import android.R;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.StrictMode;
import android.util.Log;
import com.trutruka.downloader.TrutrukaDownloaderActivity;
import android.view.View;
import android.view.WindowManager;

public class Activity extends com.unity3d.player.UnityPlayerActivity
{
    @Override
    public String getPackageCodePath() {	
            return TrutrukaDownloaderActivity.GAME_DATA_LOCAL_PATH;
    }
    
    @Override
    protected void onCreate( Bundle savedInstanceState )
    {
        super.onCreate( savedInstanceState );

        try
        {
            Class.forName( "android.os.StrictMode", true, Thread.currentThread().getContextClassLoader() );

            StrictMode.setThreadPolicy( new StrictMode.ThreadPolicy.Builder()
                    .detectNetwork()
                    .penaltyLog()
                    .build() );
        }
        catch ( Exception ex )
        {
            Log.w( "Activity", "Unable to set strict mode policy for main thread" );
            ex.printStackTrace();
        }

        final String data = "{\"via\":{\"name\":\"unity\", \"ver\":\"2.2.1\", \"platform\":\"unity\"}}";
        Muneris.boot( this, data );
        Bridge.boot(this);
        Muneris.onCreate( this );
		
    }

    
    protected void onStart()
    {
        super.onStart();
        Muneris.onStart( this );
		if (Build.VERSION.SDK_INT >= 11)
        {
            Log.i("MunerisActivity", "Setting initial lights out mode...");
            applyVisibility();
            
            final View rootView = getWindow().getDecorView();
            rootView.setOnSystemUiVisibilityChangeListener(new View.OnSystemUiVisibilityChangeListener()
            {
                @Override
                public void onSystemUiVisibilityChange(int arg0)
                {
                    if ( arg0 == 0 )
                    {
                        final Handler rehideHandler = new Handler();
                        rehideHandler.postDelayed(new Runnable() {
                            public void run()
                            {
                              Log.i("MunerisActivity", "Re-applying lights out mode...");
                              applyVisibility();
                            }
                        }, 2000);
                    }
                }
            });
        }
    }
			
    public void applyVisibility()
    {
        runOnUiThread(new Runnable()
        {
            @Override
            public void run()
            {
                try
                {
                    final View view = getWindow().getDecorView();
                    
                    final Method m = View.class.getMethod("setSystemUiVisibility", int.class);
                    m.invoke(view, 0x0); // Make it visible to work-around lockscreen resume bug
             
                    if ( Build.VERSION.SDK_INT < 14 )
                    {
                        Log.i("MunerisActivity", "Using STATUS_BAR_HIDDEN");
                        m.invoke(view, View.STATUS_BAR_HIDDEN);
                    }
                    else
                    {
                        Log.i("MunerisActivity", "Using SYSTEM_UI_FLAG_LOW_PROFILE");
                        m.invoke(view, View.SYSTEM_UI_FLAG_LOW_PROFILE); // Make it invisible
                        //m.invoke(view, View.SYSTEM_UI_FLAG_HIDE_NAVIGATION);
                    }
                }
                catch (Exception e)
                {
                    Log.w("MunerisActivity", "Cannot set lights out mode");
                    e.printStackTrace();
                }
            }
        });
    }
         
    
         

    @Override
    protected void onDestroy()
    {
        Muneris.onDestroy( this );
        super.onDestroy();
    }

    @Override
    protected void onStop()
    {
        Muneris.onStop( this );
        super.onStop();
    }

    @Override
    protected void onRestart()
    {
        super.onRestart();
        Muneris.onRestart( this );
    }

    @Override
    protected void onResume()
    {
        super.onResume();    //To change body of overridden methods use File | Settings | File Templates.
        Muneris.onResume( this );
    }

    @Override
    protected void onPause()
    {
        Muneris.onPause( this );
        super.onPause();
    }
}
