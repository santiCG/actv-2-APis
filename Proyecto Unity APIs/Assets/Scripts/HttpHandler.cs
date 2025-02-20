using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class HttpHandler : MonoBehaviour
{
    [SerializeField] GameObject[] cards;
    [SerializeField] TMP_Dropdown userDropdown;
    [SerializeField] TMP_Dropdown cardsDropdown;
    [SerializeField] UnityEngine.UI.Slider slider;
    [SerializeField] TextMeshProUGUI idText;
    RawImage pic;
    
    private int id = 1;
    private string url;
    private int indexCard;

    private void Start()
    {
        ClearCards();
        ChangeUser();
    }
    private void Update()
    {
        id = (int)slider.value;
        idText.text = $"ID: {id.ToString()}";

        indexCard = (int)cardsDropdown.value;
    }

    public void ChangeUser()
    {
        if (userDropdown.value == 0)
        {
            url = "https://rickandmortyapi.com/api/character";
        }
        else if (userDropdown.value == 1)
        {
            url = "https://my-json-server.typicode.com/santicg/actv-2-APis/characters";
        }
    }

    public void SendRequest()
    {
        StartCoroutine(GetCharacter(id));
    }

    IEnumerator GetCharacter(int id)
    {
        UnityWebRequest www = UnityWebRequest.Get(url + $"/{id}");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            if(www.responseCode == 200)
            {
                Character character = JsonUtility.FromJson<Character>(www.downloadHandler.text);

                TextMeshProUGUI[] textos = cards[indexCard].GetComponentsInChildren<TextMeshProUGUI>();
                textos[0].text = character.name;
                textos[2].text = character.status;
                textos[4].text = character.species;
                textos[6].text = character.origin.name;

                pic = cards[indexCard].GetComponentInChildren<RawImage>();
                StartCoroutine(GetImage(character.image));
            }
            else
            {
                Debug.Log($"Status: {www.responseCode}, Error: {www.error}");
            }
        }
    }

    IEnumerator GetImage(string imageUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            pic.texture = texture;
        }
    }

    public void ClearCards()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            TextMeshProUGUI[] textos = cards[i].GetComponentsInChildren<TextMeshProUGUI>();
            textos[0].text = "____";
            textos[2].text = "____";
            textos[4].text = "____";
            textos[6].text = "____";
            cards[i].GetComponentInChildren<RawImage>().texture = null;
        }
    }
}

[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public string status;
    public string species;
    public string image;
    public Origin origin;
}

[System.Serializable]
public class Origin
{
    public string name;
}
