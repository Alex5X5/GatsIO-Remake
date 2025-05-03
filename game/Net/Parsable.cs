namespace ShGame.game.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

interface Parsable<T> {
    unsafe void Serialize(byte[]* input, Player* T, int offset);
    unsafe void Deserialize(byte[]* input, Player* T, int offset);
}
