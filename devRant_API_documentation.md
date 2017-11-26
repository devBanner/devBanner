# devRant API documentation

Brief documentation of the required endpoints.

# Get UserID by Username
`GET` request to `https://devrant.com/api/get-user-id`.<br>

**Parameters**:<br>
`app=3`, `username=USERNAME`

**Responses**:<br>
Success:<br>
```
{
  "success":true,
  "user_id":ID
}
```
Fail:<br>
```
{
  "success":false
}
```

**Example**:<br>
Request:<br>
`https://devrant.com/api/get-user-id?app=3&username=dfox`<br>
Response:<br>
```
{
  "success":true,
  "user_id":487
}
```

# Get Profile by Username
`GET` request to `https://devrant.com/api/users/USER_ID`.<br>

**Parameters**:<br>
`USER_ID`, `app=3`, `content=CONTENT_SECTION`

**IMPORTANT**<br>
The `content` parameter is optional.
If it's missing or blank, the response contains the `rants` section with the `USER_ARRAY_OF_RANTS`, `upvoted` section with the `USER_ARRAY_OF_UPVOTED`, `comments` section with the `USER_ARRAY_OF_COMMENTS`, `favorites` section with the `USER_ARRAY_OF_FAVORITES` and the `collabs` section **IS MISSING**.

All the `collabs` are already present in the `USER_ARRAY_OF_RANTS`, so the `collabs` section is useful if you don't need the rest of rants. 

Examples of used `content` parameter:<br>
`content=rants`<br>
`content=upvoted`<br>
`content=comments`<br>
`content=favorites`<br>
`content=collabs`<br>
`content=rants,upvoted` **IS NOT VALID**, you can request only one section, in any other case is considered as blank.


**IMPORTANT**<br>
If the user doesn't have an avatar, the `i` property of the `avatar` section **IS MISSING**.

**Responses**:<br>
Success:<br>
```
{
  "success":true,
  "profile":{
            "username":"USERNAME",
            "score":USER_SCORE,
            "about":"USER_ABOUT",
            "location":"USER_LOCATION",
            "created_time":USER_CREATED_TIME,
            "skills":"USER_SKILLS",
            "github":"USER_GITHUB",
            "website":"USER_WEBSITE",
            "content":{
                       "content":{
                                  "rants":[{USER_ARRAY_OF_RANTS}],
                                  "upvoted":[{USER_ARRAY_OF_UPVOTED}],
                                  "comments":[{USER_ARRAY_OF_COMMENTS}],
                                  "favorites":[{USER_ARRAY_OF_FAVORITES}],
                                  "collabs":[{USER_ARRAY_OF_COLLABS}]
                                  },
                       "counts":{
                                 "rants":USER_RANTS_COUNT,
                                 "upvoted":USER_UPVOTED_COUNT,
                                 "comments":USER_COMMENTS_COUNT,
                                 "favorites":USER_FAVORITES_COUNT,
                                 "collabs":USER_COLLABS_COUNT
                                 }
                      },
            "avatar":{
                      "b":"USER_AVATAR_HEX_COLOR",
                      "i":"USER_AVATAR_IMAGE_URL"
                      },
            "avatar_sm":{
                         "b":"USER_AVATAR_HEX_COLOR",
                         "i":"USER_SMALL_AVATAR_IMAGE_URL"
                         },
            "dpp":USER_DEVRANT_SUPPORTER_FLAG
            }
}
```
Fail:
```
{
  "success":false
}
```

# Conclusions
For this project is useful to:
1. Get UserID by the Username input.
2. Get the Profile by UserID, with `content=collabs` parameter (to download the shortest possible version of the profile), and then ignore everyting except the `avatar` section.
