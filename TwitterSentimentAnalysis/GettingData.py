import tweepy, json
from datetime import timedelta
from pymongo import MongoClient


# Database connection
url = "DATABASE_CONNECTION_URL"
client = MongoClient(url)
db = client.crypto #INSTEAD OF 'CRYPTO' WRITE YOUR DATABASE NAME
dataa = db["COLLECTION_NAME"]

# Tweepy connection
consumer_key = 'YOUR_CONSUMER_KEY'
consumer_secret = 'YOUR_CONSUMER_SECRET'
access_token = 'YOUR_ACCESS_TOKEN'
access_token_secret = 'YOUR_ACCESS_TOKEN_SECRET'

auth = tweepy.OAuthHandler(consumer_key, consumer_secret)
auth.set_access_token(access_token, access_token_secret)


class getData(tweepy.StreamListener):
    def on_status(self, status):
        tweet = status._json
        if status.text.startswith('RT'):
            return True
        else:
            if status.lang == "en":
                print(status.created_at + timedelta(hours=3))  # UTC+3
                print("@" + status.author.screen_name)
                print(status.text)
                print("------------")

                crypto = {
                    'created_at': status.created_at + timedelta(hours=3),
                    'author': status.author.screen_name,
                    'tweet': status.text
                }
                if db.cryptocurrency.count_documents({"tweet": status.text}) > 0:
                    return True
                else:
                    result = db.cryptocurrency.insert_one(crypto)
            else:
                return True


data = getData()
stream = tweepy.Stream(auth, data)

def streamFilter():
    stream.filter(track=["Cryptocurrency"])


streamFilter()

