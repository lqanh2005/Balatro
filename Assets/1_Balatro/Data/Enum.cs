using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IngredientType
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
    None,
    Ingredient,
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

public enum Voucher
{
    None,
    TheHermit, //x2 gold
    TheWorld, //Clone Ingredient
    Temperance, // 0-30% ngẫu nhiên đạt được 0-30% target hiện tại
    Judgement, //RandomIngredient
    TheFool, // random upgrade 1 công thức
    TheEmperor, // random 2 voucher
    BlackFriday, //discount đồ trong shop
    TheMagician, // Nâng cấp ngẫu nhiên 1 nguyên liệu lên 1 bậc
}

public enum ItemType
{
    Ingredient,
    Recipe,
}