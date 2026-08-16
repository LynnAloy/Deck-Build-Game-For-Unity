return {
    version = 0,

    cards = {
        fireball = {
            mana = 3,

            CalculateDamage = function(baseDamage, targetHpPercent)
                if targetHpPercent < 0.3 then
                    return baseDamage * 2
                end

                return baseDamage
            end
        }
    }
}