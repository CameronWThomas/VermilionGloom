using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterCustomizer : MonoBehaviour
{
    //TODO looks like there are some areas where we want matching materials. This randomizes for everything.
    //     could go back and fix that if wanted

    [SerializeField] SkinnedMeshRenderer _bow;
    [SerializeField] List<SkinnedMeshRenderer> _fDress1;
    [SerializeField] List<SkinnedMeshRenderer> _fDress2;
    [SerializeField] List<SkinnedMeshRenderer> _fHair;
    [SerializeField] List<SkinnedMeshRenderer> _m1Dressy;
    [SerializeField] List<SkinnedMeshRenderer> _m1Fancy;

    [SerializeField] List<SkinnedMeshRenderer> _shoeMeshes;
    [SerializeField] List<SkinnedMeshRenderer> _skinMeshes;

    [SerializeField] List<Material> _fSkinMats;
    [SerializeField] List<Material> _mSkinMats;
    [SerializeField] List<Material> _fHairMats;
    [SerializeField] List<Material> _outfitMats;
    [SerializeField] List<Material> _bootMats;

    public void DressRandomly()
    {
        DisableAllMeshRenderers();

        if (CoinFlip())
            DressMaleCharacterRandomly();
        else
            DressFemaleCharacterRandomly();
    }

    private void DressMaleCharacterRandomly()
    {
        var outfitComponents = CoinFlip() ? _m1Dressy : _m1Fancy;

        DressCharacterRandomly(outfitComponents, _mSkinMats);
    }

    private void DressFemaleCharacterRandomly()
    {
        if (CoinFlip())
        {
            _bow.enabled = true;
            _bow.material = _outfitMats.Randomize().First();
        }

        var hair = _fHair.Randomize().First();
        hair.enabled = true;
        hair.material = _fHairMats.Randomize().First();

        var outfit = CoinFlip() ? _fDress1 : _fDress2;

        DressCharacterRandomly(outfit, _fSkinMats);
    }

    private void DressCharacterRandomly(List<SkinnedMeshRenderer> outfitComponents,
        List<Material> skinMats)
    {
        var skinMat = skinMats.Randomize().First();

        foreach (var outfitComponent in outfitComponents)
        {
            outfitComponent.enabled = true;

            if (_skinMeshes.Contains(outfitComponent))
                outfitComponent.material = skinMat;
            else if (_shoeMeshes.Contains(outfitComponent))
            {
                var bootMat = _bootMats.Randomize().First();
                outfitComponent.material = bootMat;
            }
            else
            {
                var materials = outfitComponent.materials
                    .Select(_ => _outfitMats.Randomize().First())
                    .ToList();
                outfitComponent.SetMaterials(materials);
            }
        }
    }

    private void DisableAllMeshRenderers()
    {
        var meshRenderers = _fDress1.Concat(_fDress2)
            .Concat(_fHair)
            .Concat(_m1Dressy)
            .Concat(_m1Fancy)
            .ToList();

        meshRenderers.Add(_bow);

        meshRenderers.ForEach(x => x.enabled = false);
    }

    private bool CoinFlip()
        => UnityEngine.Random.Range(0, 2) == 1;
}