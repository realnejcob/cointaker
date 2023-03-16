using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class EdgeTextureSelector : MonoBehaviour {
    [SerializeField] private Sprite[] edgeTextures;
    private Image image;

    private void Awake() {
        image = GetComponent<Image>();
    }

    private void Start() {
        image.sprite = edgeTextures[Random.Range(0, edgeTextures.Length)];
        var newRot = (Random.Range(0, 2) == 0) ? Quaternion.Euler(Vector3.forward * 180) : Quaternion.Euler(Vector3.zero);
        transform.rotation = newRot;
    }
}
