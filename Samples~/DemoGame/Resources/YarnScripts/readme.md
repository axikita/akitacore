Yarn files need to go in a folder named "resources" so they can be dynamically loaded with 
Resources.Load<TextAsset>("filename");

whitespace in [[name|destination here]]
\[\[.*\|.+([ ]).+\]\]

whitespace in [[destination here]]
\[\[.*([ ]).+\]\]

3 group: 1 = initial, 2= this string needs the first letter capilized. [[word1 word2]]
((?:.|[\n])*\[\[\w+ )(\w+)(]\](?:.|[\n])*)

((?:.|[\n])*\[\[\w+[ ])([a-z])(.*]\](?:.|[\n])*)


as above, for [[word|word1 2]]
((?:.|[\n])*\[\[\w+\|\w+[ ])(\w+)(]\](?:.|[\n])*)

((?:.|[\n])*\[\[\w+\|\w+[ ])([a-z])(.*]\](?:.|[\n])*)