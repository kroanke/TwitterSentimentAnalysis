from textblob import TextBlob
from nltk.sentiment.vader import SentimentIntensityAnalyzer
import re

from pymongo import MongoClient

##database connection
url = "DATABASE_CONNECTION_URL"
client = MongoClient(url)
db = client["DATABASE_NAME"]
data = db["COLLECTION_NAME"]

#Cleaning tweets from unnecessary characters
def clean_tweet(tweets):
    tweets = re.sub("@[A-Za-z0-9_]+", "", tweets)
    tweets = re.sub("#[A-Za-z0-9_]+", "", tweets)
    tweets = re.sub("$[A-Za-z0-9_]+", "", tweets)
    tweets = re.sub(r"http\S+", "", tweets)
    tweets = re.sub(r"www.\S+", "", tweets)
    tweets = re.sub('[()!?]', ' ', tweets)
    tweets = re.sub('\[.*?\]', ' ', tweets)
    tweets = re.sub(r'^https?:\/\/.*[\r\n]*', '', tweets)
    tweets = re.sub("([^0-9A-Za-z \t])|(\w+:\/\/\S+)", "", tweets)
    return tweets


def getTweet():
    collectionTweet = data.find({}, {'_id': 0, 'tweet': 1})
    tweets = []

    for tweet in collectionTweet:
        tweets.append(tweet["tweet"])
    return tweets


def analyze(tweets):
    polarity = 0
    tweet_list = []
    neutral_list = []
    negative_list = []
    positive_list = []
    for tweet in tweets:
        tweet_list.append(tweet)
        analysis = TextBlob(tweet)
        score = SentimentIntensityAnalyzer().polarity_scores(tweet)
        neg = score['neg']
        neu = score['neu']
        pos = score['pos']
        comp = score['compound']
        polarity += analysis.sentiment.polarity
        if neg > pos:
            print("neg")
            negative_list.append(tweet)
            db.cryptocurrency.update_one({'tweet' : tweet}, {"$set" : {"polarity": -1}})

        elif pos > neg:
            print("pos")
            positive_list.append(tweet)
            db.cryptocurrency.update_one({'tweet' : tweet}, {"$set" : {"polarity": 1}})

        elif pos == neg:
            print("neu")
            neutral_list.append(tweet)
            db.cryptocurrency.update_one({'tweet' : tweet}, {"$set" : {"polarity": 0}})
analyze(getTweet())

