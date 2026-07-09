using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticTexture
{
    private const string Circle = "iVBORw0KGgoAAAANSUhEUgAAAIAAAACACAYAAADDPmHLAAAFMElEQVR4Ae2d2XJiMQxEJ1P5/18mZVIixtzFi2S15Z6XQACrdfqUydt8PR6Pf8H+WS/0FYnXd4BlrAsvEZXzlhZiRQHKAsqCZj8v8ywlxCoClJBnl9wyL88KLwO6ADnMlhJQ3iv5YUVAFECgoZSokSPfCUoGJAFySBrQUc+QPSFEQBBAgKAWZpVL9nYV4b/VdpXnCoTKt4d8mysDrxvAdWlAjYTH9NvA4waQZQF7cI80nc3MG2D6cu519gUQTlNug1k3gCzVh2TPT01hNkOAKYsEdcScneVXgHn4oKWXawlHk68EqxtAQpfL8Hk/AROmFgKYBO3nFuqT6my1BVAPGKo+nWVUGWsKoBpMh1XYU9RYawmgFihsZfqLqTDXEEAliD6fLU4cZj8qwHCALWqyXXKogxEBhgbbMtnu9O4uRgTYjnLEhXsF6DYuIkSQnbo66RGgaxAIpOgxmrtpFaB5QHTigPs1ddQqAOC+jDRCoEWAJrNGQvGzwwSqu6oVoPrA4eg8QItAVWe1AmiF4jlgBGoEqDIJbC/G+SVw292dALcHkDQ8gcsO7wSA344BxwhcCXBpzthYfnoygdMurwSYnJHjPAicCXBqjEdIzlQhcNjpmQAqE3kIPgEKgN+RacIjAQ6vCtMUPHwWgY9ujwSYFYZzAAiUAnwYApCREXQJvHVcCqA7iqfBE6AA8BXZBswFeLsabMfydGcCr65zAZwzcbwHAQrgQR1oJgUAKsMjigjw+k7wCMGZLgSenYsALgk41J8ABfDvwDUBBXDF7z88CcDvf/8evBI8eAN4oQeZSwFAivCKQQG8yIPMpQAgRXjFoABe5EHmUgCQIrxiUAAv8iBzKQBIEV4xKIAXeZC5FACkCK8YFMCLPMhcCgBShFcMCuBFHmQuBQApwisGBfAiDzKXAoAU4RWDAniRB5lLAUCK8IpBAbzIg8ylACBFeMWgAF7kQeZSAJAivGJQAC/yIHOTACb/KzXIfoxxTeCLN8A1oPCvUoDwFV8vSAGu+YR/VQTg3wHhq/5Y8Nm5CPDxKn+xBwEKsEfPp1tSgFM0e7yQC8C/A/boPG356joXYJ/1uemLAAV4odjzQSnA62rYE8cWW791XAqwBQEu+UfgSIA3Q/7eykcBCHx0eyRAgD25Qi0BClBLKuj7zgT4uCqC7r/TWoedngmwE5itd70S4NCYrWmtu/xpl1cCrLsuk1cTuBPg1JzqCXyjN4HLDu8ESOEvD/DejvMvCdx2VyPA5QS+uDaBWgFuTVobQ8j0VZ3VCpAIVR0YEuV6S1V31SLAehiY+JZAqwDVZt1O5husCDR11CpACt00wGpLnntIoLmbHgHS5OZBh3H5S00CXZ30CqAZnGc5EhgRoMs4x10jj+7uYkSABLR7cOQ2Ju821MGoAJRgctvFuKHy01kaAqRzhoOkQ/iviYAKcy0BUnKVQE0I9n2zGmtNASjBHCHVyk9xtQWgBLYSqJZvJQAlsJFAvfwU89sm6/NUCcz/nXwMsnAcO+Xk0xZfAeUo0wXKYcGem7ObIUDqxHyRYMVPY2b5FVB2IhLwK6Ek8/5cOL3/1ujZrBsgjz91wXzwAo+ns5l5A+T8ZVHeBr9UhEfOaMpjjxsgX8xt8TyE82NXBl43QM5cAOx2G8jeOYvpjxEEkKUFSHQRZE/Z2/UnkgACIgcURYZ8J9kT4ieiADkYAbeqCJI/3wnqMboAAisHiS5DnlXyw/5cRYAcYAnYW4gyT54V/vGKApRQywKshSjnlXmWev4DcS9XJY5zxOMAAAAASUVORK5CYII=";

    private const string Ring = "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";

    private const string Indicate =
        "iVBORw0KGgoAAAANSUhEUgAAAIAAAACACAYAAADDPmHLAAADVElEQVR4Ae2cQY7iUAxEm1Hfq7lZZ242c7JMsrGoBS2MBPPl9yJFsj9GcVU9wS6Xfd+vHx8f573yta283A+7Lb/35QDgXPL7BxErfHRZYYkndtif+M5bv/LrrU/zYcs5IADLRfLehQTgvX4v97TPOxv9Oc7/3vnM48cd+P346Fsmv46nXG+fdA+AM/ztdtD6KQdW8/Dc53rcdfkXUFYwCwFg5l6qBaCsYBYCwMy9VAtAWcEsBICZe6kWgLKCWQgAM/dSLQBlBbMQAGbupVoAygpmIQDM3Eu1AJQVzEIAmLmXagEoK5iFADBzL9UCUFYwCwFg5l6qBaCsYBYCwMy9VAtAWcEsBICZe6kWgLKCWQgAM/dSLQBlBbMQAGbupVoAygpmIQDM3Eu1AJQVzEIAmLmXagEoK5iFADBzL9UCUFYwCwFg5l6qz/cEVmPBc8BfAF7moVgAwg5eIwC8zEOxAIQdvEYAeJmHYgEIO3iNAPAyD8UCEHbwGgHgZR6KBSDs4DUCwMs8FAtA2MFrBICXeSgWgLCD1wgAL/NQLABhB68RAF7moVgAwg5eIwC8zEOxAIQdvEYAeJmHYgEIO3iNAPAyD8UCEHbwGgHgZR6KBSDs4DUCwMs8FAtA2MFrBICXeSgWgLCD1wgAL/NQLABhB68RAF7moVgAwg5eIwC8zEOxAIQdvEYAeJmHYgEIO3iNAPAyD8UCEHbwGgHgZR6KBSDs4DUCwMs8FAtA2MFrBICXeSg+AdiO+3xl+O19nnnNc2A7JN3mvPsLMC/kliIBaNk1b1gA5mXaUiQALbvmDQvAvExbigSgZde8YQGYl2lLkQC07Jo3LADzMm0pEoCWXfOGBWBepi1FAtCya96wAMzLtKVIAFp2zRsWgHmZthQJQMuuecMCMC/TliIBaNk1b1gA5mXaUiQALbvmDQvAvExbigSgZde8YQGYl2lLkQC07Jo3LADzMm0pEoCWXfOGBWBepi1FAtCya96wAMzLtKVIAFp2zRsWgHmZthQJQMuuecMCMC/TliIBaNk1b1gA5mXaUvR5Z/rrON/ufPY/jlfapaN/tb3PXOO67Pu+HSffcbpec1lvpYc2Ol/ItPTlX8DS8bx+OQF4vcdLP0EAlo7n9cv9A9rXIu1zGh9YAAAAAElFTkSuQmCC";


    private const string HexaHorizontal =
        "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";

    private const string HexaVertical =
        "iVBORw0KGgoAAAANSUhEUgAAAIAAAACACAYAAADDPmHLAAAHvElEQVR4Ae1bi3XjNhD05V0BSgdwB7oOqA6cDuQOnArOriB3FZw7iDswOzh1EHQQduDMKIGiD0WRID4kMfseDFIEsLuzy8FH8qePj487SbkI/FKu6/KcCCgBCs8DJYASoHAECndfDKAEKByBwt0XAxSeAJ8n4v8D7FhPxJZUZuyg6C2Vsmt6cicAg/4nirlm4MI/t/DvNxQmQxb5lPEksILH71m8np7SDUyqc5iVMwH+gsMmh9MT1Glh030Ou3ItAp/hrMnh8ER1EovnHLblYIAVHOXbz1ryPwINLskCrJNJDgb4A94p+JchJibEJqmkZoA1vPuZ1MP5KfsCk3epzE7NAMkzPBWQAfUkxShlAmwBUhUQqKUORYy2qZxLNQVwfiP1m1SOzVyPhf2cCprYfqRigCc4YmI7s6DxiRUxiy4pGMDAC779ZAFJfwT49pMFbP8uw1umYAAuahT84bEhZtEXhLEZoIITOu8fHvzjHhvc1McfhLyOnQCk/nVIgwscawefORVEkZhTwBYWK/jjw0YMiWUUicUAK1ir8/5wIWswVJTvCWIxwBMMZhJIwiBALIlpcInBAAZW8u2XhEeALGBDDhuDAX6ENFBjnSAQHNvQCVDBXBZJHAQqDMsSTEJPAaR+E8w6DdSGgMWHnAqCSEgG4CLFBLFKg3QhQIyDLQhDMQBXqXz7WUviI9BABVmA9SgJxQBfYYWCPyoUgzoTa2I+WkIwwBpW8MhXkh6B0T8fC8EA0b+xSo/rbDSOxn5sAjwAqmo2cC3PUGLPGHjL2ClA2z5v6IN1tBjJe1s4hgGeodigSPIiYKD+2dcEXwbgKlTbPl/Uw/drMKTXttCXAbj4YBJIpoEAY+G1IPRhgArK3qfht6w4Q2CD+/rss85bHwYIcgDRaZUe+iIwODZDE2ALyypf69QvOgKMzXaIliFTAOeZnyhmiAK1TY6Ahcbe/1U0hAGeMLBBkUwbAQPzGKte0pcBDEbjtk8yHwS4LbS3zO3LAF5bjFvK9TwqAr1i1ocBKpipbV/UWEUb/Oa2sE8CcOG3jmaiBo6JwA6Dd/5X0a0pYIsBFPyYIYo7NmO37VLRxQArdOTCj7Vkvgg0MJ0LQtYX0sUAPFVS8C8gm90HjCFj2SrXGMCgNd9+yXIQaN0WXmOAH8vxW578h0BrTNsSoEIHFsmyEKjgDsuJtE0BpH5z0ko3S0HAwhFOBQc5Z4AnPDGHp7pYGgKMLWN8kGMG4GqRbz9ryXIRaOAaWYD13TED8OxYwScqyxbG+PA9gWOANT7kka+kHAT2/1XkGOCQEeX4X7yn+5iTAUgJfxcPR5kA/EoGIP1LykRg7aaAMt2X13eaAspOgv0U0ACDumwcivSeMW+0DSwy9nunT7aBO3z0Wi4WxXnOWDPm+zUAa8oKRUfBeygW/YdTfutRMB+8LNp1OUcEGGPGei9uDeDuWevr4GM0lnVt4Q7f/oO0nQM8Hp7qYmkIXMS2LQFqeM0iWRYCNdxhOZG2KYANDAqnAslyECD123N32hiAbdjwGy8ki0CAsbRtnlxjALZdoWhb2IbavD7jip9vP+sLucYAbMgOv1/00AdzQ4AxbA0+HeliAOcofym0djeqZ4XADtbyyPeqdDGA6yQWcEjMr74Zuz4JUMPvt/n5XrzFjFl9C4U+UwDHMCjaFhKJ+Ujrtu/c/D4MwD4W5YUXklkgwFjZPpb2ZQCOtULhgtCgSKaLgIVpXPg1fUzsywAciwOKBfqgmrcNY9Qr+DRzCAM4t95xUbkb1ZNCoIY1myEWDWEAN65YwCExvXpwbHwSoIbfr9PzvXiLGJN6KAo+UwB1cEGo7wmGoh2vPef8e5Tec78zxYcB2JeKvrtBVGdHgLEYHHxa7csAzmOygHE3qrMgYKGVb7+X+DKAU3bzrNk1VB0NgVExGMsA9ErbwmixvTlwjRabm606GoRIgDXG5wmhJD0CX6ByN0bt2CmAumnAtzFGqK8XAsR8VPCpNQQDcJwViraFRCKNNFDjte07Ny8EA3BMGvRyPrjuoyFArIn5aAnFAM4QbQsdEvFqi6H59geRUAzgjHl0F6qjIRAU49AJUMNtFkkcBGoMyxJMQk8BNMygcCqQhEeA1G9DDhuaAWgbDXzhhSQoAsTUBh0Rg8VgANqobWHYSAXb9p2bFYMBqIMGjzqjPje08HtiGWTbd45jLAZwevRfRQ4J/3qHrjzyjSKxGMAZKxZwSPjXUTGMnQA1/H7z9734nsSujolC7CmAthsUTgUrFEl/BBo0JfXb/l2Gt4zNALTIonznhWQQAsTMDurh0TgFA9Asvv36r6L+AbJoyrefLBBVUjAAHaAjOhzqH0piFT34NCcVAzjX9fMxh8T1usajzfXHYZ+kToA1zNfPx7pjSOrfdTcJ9zTVFOAspmOv7kb1BQLEJlnwqT01A1DnCkU/HyMSp9Lg9h6FdTJJzQB0jA5qW3gZYmKSNPg0IQcDONf18zGHxL/7fb79ySUHAzgnH92F6rtsWORMgBqB54rXopQqFo4Tgxoli+ScAo4dfsDN+viDAq538PEtt59TSYDcOBSrP+cUUCzoU3JcCTClaGSwRQmQAfQpqVQCTCkaGWxRAmQAfUoqlQBTikYGW5QAGUCfksp/AJhShQjYW50VAAAAAElFTkSuQmCC";

    private const string Triangle =
        "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";

    private const string TriangleReverse =
        "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";
        
        
    private static Texture2D _circleTexture;
    private static Texture2D _ringTexture;
    private static Texture2D _indicateTexture;
    private static Texture2D _hexaHorizontalTexture;
    private static Texture2D _hexaVerticalTexture;
    private static Texture2D _triangleTexture;
    private static Texture2D _triangleReverseTexture;
  
//    private Texture2D _toSerialTexture;
//    
//    public void OnGUI()
//    {
////        _toSerialTexture = (Texture2D) EditorGUILayout.ObjectField("Texture2D", _toSerialTexture, typeof(Texture2D), true);
////        if (GUILayout.Button("Serialize Texture2D", new GUILayoutOption[0]))
////            File.WriteAllText ($"Assets/{_toSerialTexture.name}.txt", StaticTexture.Texture2DToBase64(_toSerialTexture));


    public static Texture2D TriangleTexture
    {
        get
        {
            if (_triangleTexture == null)
            {
                _triangleTexture = Base64ToTexture2D(Triangle);
            }

            return _triangleTexture;
        }
    }
    
    public static Texture2D TriangleReverseTexture
    {
        get
        {
            if (_triangleReverseTexture == null)
            {
                _triangleReverseTexture = Base64ToTexture2D(TriangleReverse);
            }

            return _triangleReverseTexture;
        }
    }


    public static Texture2D CircleTexture
    {
        get
        {
            if(_circleTexture == null)
            {
                _circleTexture = Base64ToTexture2D(Circle);

            }

            return _circleTexture;
        }
    }
    
    public static Texture2D RingTexture
    {
        get
        {
            if(_ringTexture == null)
            {
                _ringTexture = Base64ToTexture2D(Ring);
            }

            return _ringTexture;
        }
    }
    
    public static Texture2D IndicateTexture
    {
        get
        {
            if(_indicateTexture == null)
            {
                _indicateTexture = Base64ToTexture2D(Indicate);
            }

            return _indicateTexture;
        }
    }
    
    public static Texture2D HexaHorizontalTexture
    {
        get
        {
            if(_hexaHorizontalTexture == null)
            {
                _hexaHorizontalTexture = Base64ToTexture2D(HexaHorizontal);
            }
     
            return _hexaHorizontalTexture;
        }
    }   
    
    public static Texture2D HexaVerticalTexture
    {
        get
        {
            if(_hexaVerticalTexture == null)
            {
                _hexaVerticalTexture = Base64ToTexture2D(HexaVertical);
            }
     
            return _hexaVerticalTexture;
        }
    }

      
    static Texture2D _squareTexture;
    public static Texture2D SquareTexture
    {
        get
        {
            if (_squareTexture == null)
            {
                _squareTexture = new Texture2D(1, 1, TextureFormat.ARGB4444, false);
                _squareTexture.SetPixel(0, 0, Color.white);
                _squareTexture.Apply();
            }

            return _squareTexture;
        }
    }
    
    static Texture2D _squareTextureBlue;
    public static Texture2D SquareTextureBlue
    {
        get
        {
            if (_squareTextureBlue == null)
            {
                _squareTextureBlue = new Texture2D(1, 1, TextureFormat.ARGB4444, false);
                _squareTextureBlue.SetPixel(0, 0, Color.blue);
                _squareTextureBlue.Apply();
            }

            return _squareTextureBlue;
        }
    }

    public static string Texture2DToBase64(Texture2D texture)
    {
        byte[] imageData = texture.EncodeToPNG();
        return Convert.ToBase64String(imageData);
    }
		
    public static Texture2D Base64ToTexture2D(string encodedData)
    {
        byte[] imageData = Convert.FromBase64String(encodedData);
			
        int width, height;
        GetImageSize(imageData, out width, out height);
			
        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false, true);
        texture.hideFlags = HideFlags.HideAndDontSave;
        texture.filterMode = FilterMode.Point;
        texture.LoadImage(imageData);
			
        return texture;
    }
    
    private static void GetImageSize(byte[] imageData, out int width, out int height)
    {
        width = ReadInt(imageData, 3 + 15);
        height = ReadInt(imageData, 3 + 15 + 2 + 2);
    }
		
    private static int ReadInt(byte[] imageData, int offset)
    {
        return (imageData[offset] << 8) | imageData[offset + 1];
    }
}
