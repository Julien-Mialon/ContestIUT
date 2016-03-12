using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contest.Model
{
    public class MessageHeader
    {
	    public const int LOGIN_PLAYER = 0;
	    public const int LOGIN_ACK = 2;
	    public const int LOGOUT = 3;
	    public const int KICK = 4;
	    public const int WELCOME = 5;
	    public const int GAME_STARTS = 6;
	    public const int GAME_ENDS = 7;
	    public const int TURN = 8;
	    public const int TURN_ACK = 9;
    }
}
