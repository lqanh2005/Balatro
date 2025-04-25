using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    RICE = 1,
    MEAT = 2,
    FLOUR =3,
    EGG = 4,
    VEGETABLE = 5,
    CHEESE = 6,
    SAUSAGE = 7,
}

public enum Recipe
{
    StrickyRice = 10, // xoi
    CoinPancake = 11, // banh dong xu
    RiceNoodles = 14, // bun
    BaoZi = 13, 
    HotDog = 17,
    Ramen = 18,
    Salad = 21,
    Tokbokki = 9,
    FriedRice = 19,
    VietnamesePizza = 25,
    BanhMi = 27,
    StoneSausage = 33, // lap xuong nuong da
}

public enum RankCard
{
    Comon,
    Silver,
    Gold,
    Diamond,
}