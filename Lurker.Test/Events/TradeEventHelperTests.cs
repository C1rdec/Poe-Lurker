//-----------------------------------------------------------------------
// <copyright file="TradeEventHelperTests.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Test.Events
{
    using Lurker.Helpers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TradeEventHelperTests
    {
        [TestMethod]
        [DataRow("@QuestionableStormBruh Hi, I would like to buy your level 1 0% Vaal Detonate Dead listed for 1 jew in Delirium (stash tab \"Q\"; position: left 15, top 12)")] // English
        [DataRow("@날려보자고 안녕하세요, 환영(보관함 탭 \"판매\", 위치: 왼쪽 11, 상단 10)에 39 chaos(으)로 올려놓은 레벨 1 0% 계몽 보조(을)를 구매하고 싶습니다")] // Korean
        [DataRow("@Сардикс Здравствуйте, хочу купить у вас уровень 1 0% Усилитель за 19 chaos в лиге Делириум (секция \"Shop\"; позиция: 1 столбец, 2 ряд)")] // Russian
        [DataRow("@xหวัง๔เจ้าZzzz  สวัสดี, เราต้องการจะชื้อของคุณ level 2 0% Empower Support ใน ราคา 20 chaos ใน Delirium (stash tab \"฿฿฿\"; ตำแหน่ง: ซ้าย 17, บน 18)")] // Thai
        [DataRow("@strk_del_iceshot Bonjour, je souhaiterais t'acheter Joyau cobalt dans la ligue Delirium (onglet de réserve \"trade\" ; 8e en partant de la gauche, 1e en partant du haut)")] // French
        [DataRow("@strk_del_iceshot Hi, ich möchte 'Diamantring' in der Delirium-Liga kaufen (Truhenfach \"trade\"; Position: 11 von links, 1 von oben)")] // German
        [DataRow("@strk_del_iceshot Olá, eu gostaria de comprar o seu item Joia Cobalto na Delirium (aba do baú: \"trade\"; posição: esquerda 10, topo 1)")] // Portuguese
        [DataRow("@strk_del_iceshot Hola, quisiera comprar tu Ídolo  Sombrío Amuleto de Ámbar en Delirium (pestaña de alijo \"trade\"; posición: izquierda10, arriba 2)")] // Spanish
        public void GivenATradeEventHelper_WhenCheckingIfInputIsTradeMessage_ThenIsTradeMessage(string input)
        {
            // Act
            bool isTradeMessage = TradeEventHelper.IsTradeMessage(input);

            // Assert
            Assert.IsTrue(isTradeMessage);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("This is not a trade message")]
        [DataRow("@something something something")]
        public void GivenATradeEventHelper_WithOtherInput_WhenCheckingIfInputIsTradeMessage_ThenIsNotTradeMessage(string input)
        {
            // Act
            bool isTradeMessage = TradeEventHelper.IsTradeMessage(input);

            // Assert
            Assert.IsFalse(isTradeMessage);
        }
    }
}
