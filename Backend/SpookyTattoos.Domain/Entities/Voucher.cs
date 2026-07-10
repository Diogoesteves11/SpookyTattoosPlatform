/*
Copyright 2026 Diogo Esteves, Guilherme Mattos

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/


namespace SpookyTattoos.Domain.Entities;

public class Voucher
{
    public int Id { get; set; }
    
    public required int EmitterId { get; set; } 
    public Client? Emitter { get; set; }

    public required decimal Value { get; set; }
    
    public bool IsUsed { get; set; } = false;
    
    public DateTimeOffset GeneratedAt { get; set; } = DateTimeOffset.UtcNow;
    
    public DateTimeOffset ExpiresAt { get; set; } = DateTimeOffset.UtcNow.AddYears(1);

    public bool IsExpired()
    {
        return DateTimeOffset.UtcNow > ExpiresAt;
    }
}