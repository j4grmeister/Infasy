using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeperAI : CharacterAI {
	public Shop shop;
	ushort townID = 0;
	ushort shopID = 0;

	protected override void ControllerStart () {
		townID = (ushort)character.characterInfo.Get ("townID");
		shopID = (ushort)character.characterInfo.Get ("shopID");
		shop = (Shop)Game.current.map.GetTownByID (townID).GetBuildingByID (shopID);
	}

	protected override void ControllerUpdate () {

	}

	public override void OnPlayerInteract () {
		//Game.dialogueMenu.Say (character, "Oh! Hi there! You should buy some weapons so that I can stay in business.");
		System.Action buy = () => {
			//Game.dialogueMenu.Say (character, "Thanks! What would you like to buy?");
			Game.shopMenu.Buy (townID, shopID);
		};
		System.Action sell = () => {
			Game.shopMenu.Sell (townID, shopID);
		};
		System.Action bye = () => {
			
		};
		character.Prompt ("hi there! how can i help you?", new string[] { "buy", "sell", "goodbye" }, new System.Action[] { buy, sell, bye });
	}
}

/*
 * EASTER EGGS!!!!
 * The Adventure Zone
 * Garfield:
 * Angling to make a dealllllllllllllllllll?
 * Sweeten the pot a little!
 * 
 * The Slicer of T'pire Weir Isles (900 GP):
 * A stone which, on a successful persuasion check, can be traded
 * to anyone for the most valuable thing they have in their possession.
 * 
 * 
 * Rick Axager's Guide to Adventuring 3rd Ed. (1500 GP):
 * Once a day, the user can read a section of an associated skill check.
 * For the next 24 hours the user has advantage on that check.
 * The user can also read the guide aloud to give the advantage to another party member.
 * 
 * Flaming Poisoning Raging Sword of Doom (60,000 GP):
 * 
 * Drawmij's Instant Summons
 * 
 * Taako:
 * *read section on persuasion from Guide to Adventuring*
 * I have one more transaction I'd like to conduct.
 * 
 * Garfield: Okay
 * 
 * Taako: Garfield.
 * 
 * Garfield: Yeaass.
 * 
 * Taako:
 * I have something that I think is really going to interest you....
 * *Griffin yells "Oh my God"* 
 * This is the Slicer of T'pire Weir Isles... and I notice that you have a really cool sword.
 * It's a Flaming Poisoning Raging Sword of Doom...? I believe it's called.
 * And I'm looking at your entire stock and it does seem to me that's your most valuable possesion. Would you say that's accurate?
 * 
 * Garfield: Yes! It's absolutely the most valuable thing in the store!
 * 
 * Taako:
 * Well get ready to talk about that in the past tense, my man, because I have got something really special for you.
 * This is, number one, an exotic item. I know people are always looking for those.
 * You can't buy this at any store around the block; in fact, as far as I know, there...
 * 
 * Garfield: mmmm. From a far away land! It smells of exotic spices!
 * 
 * Taako:
 * Oh, I imagine. I am very impressed, you know, not a lot of people have this sort of old factory accuteness able to sense that.
 * You, uh, must be a very discerning smeller. Uh, so this is a very valuable item, and trust me when I say it is going to pay big, big dividends for you.
 * Uh, uh, uh, uh, If we could just make this transaction. This is... As much as this pains me to say, I have come here and I only a have this to offer.
 * I have no gold. And I say it pains me because you're getting such a good deal off of me, but I do need the sw...
 * 
 * Garfield: But my thing costs 60,000 G...Ps. So... how many GPs is yours?
 * 
 * Taako: Get ready for this. 61,000. Can you believe it? What a steal, eh?
 * 
 * Garfield: That's quite a profit!
 * 
 * Taako: Mmhmm. I know that's what you're all about, is profit. Hmm?
 * *rolls a 2*
 * *rolls an 18*
 * Travis: I would like to point out, Griffin. 18 is the third best number he could roll.
 * *Magnus darts his eyes back and forth while this is happening*
 * 
 * Garfield:
 * Let me smell it.
 * *takes the item and smells it*
 * Mmm. Those spices though.
 * *gives it a little taste*
 * Mmm. All my senses are delighted by this bad boy.
 * ...
 * Yeah, okay! Sounds fair to me!
 * 
 * Taako: Nice!
 * 
 * Garfield:
 * Sucker. You have no idea what you've just done, do you?
 * 
 * Taako: No.?
 * 
 * Garfield:
 * You've just made a bad trade. This stone you said was worth 61,000 GP is worth, easily double that!...
 * In the hands of a brilliant merchant like myself, Garfield!
 */