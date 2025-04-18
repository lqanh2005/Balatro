using Google.Play.Common;
using Google.Play.Review;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MyReviewManager : MonoBehaviour
{
    // Create instance of ReviewManager
    private ReviewManager _reviewManager;
    PlayAsyncOperation<PlayReviewInfo, ReviewErrorCode> playReviewInfoAsyncOperation;
    PlayAsyncOperation<VoidResult, ReviewErrorCode> playReviewInfoAsyncOperation_2;

    public void ShowReview(Action actionErorr)
    {
        _reviewManager = new ReviewManager();
        playReviewInfoAsyncOperation = null;

        playReviewInfoAsyncOperation = _reviewManager.RequestReviewFlow();
        playReviewInfoAsyncOperation.Completed += playReviewInfoAsync => Complete(playReviewInfoAsync, actionErorr);
    }

    void Complete(PlayAsyncOperation<PlayReviewInfo, ReviewErrorCode> playReviewInfoAsync, Action actionErorr)
    {
        if (playReviewInfoAsync.Error == ReviewErrorCode.NoError)
        {
            // display the review prompt
            var playReviewInfo = playReviewInfoAsync.GetResult();
            playReviewInfoAsyncOperation_2 = _reviewManager.LaunchReviewFlow(playReviewInfo);

            playReviewInfoAsyncOperation_2.Completed += playReviewInfoAsync => Complete2(playReviewInfoAsync, actionErorr);
        }
        else
        {
            if (actionErorr != null)
            {
                actionErorr();
            }    
          
            // handle error when loading review prompt
            Debug.Log("Erro Review Inapp " + playReviewInfoAsync.Error);
        }
    }

    void Complete2(PlayAsyncOperation<VoidResult, ReviewErrorCode> playReviewInfoAsync, Action actionErorr)
    {
        if (playReviewInfoAsync.Error == ReviewErrorCode.NoError)
        {
            if (actionErorr != null)
            {
                actionErorr();
            }
        }
        else
        {
            if (actionErorr != null)
            {
                actionErorr();
            }
        }    


    }    
}