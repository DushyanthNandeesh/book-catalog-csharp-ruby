# frozen_string_literal: true

require_relative 'book'

# Input rules from the shared specification.
# Used by both the menu and the file loader.
module Validation
  MIN_YEAR = 1000
  MAX_INT = 2_147_483_647 # same 32-bit limit the C# version gets from int

  module_function

  # Text fields must contain something other than whitespace.
  # Ruby has no static types, so we also check that the value really is a String.
  def valid_text?(value)
    value.is_a?(String) && !value.strip.empty?
  end

  # Year must be a whole number from 1000 through the current year.
  def valid_year?(year)
    year.is_a?(Integer) && year.between?(MIN_YEAR, Time.now.year)
  end

  # Digits only (no sign, no decimals). Returns an Integer, or nil if invalid.
  def parse_int(text)
    cleaned = text.to_s.strip
    return nil unless cleaned.match?(/\A\d+\z/)

    number = cleaned.to_i
    number <= MAX_INT ? number : nil
  end

  # Checks a book that was read from the JSON file.
  def valid_book?(book)
    book.is_a?(Book) &&
      book.id.is_a?(Integer) && book.id.between?(1, MAX_INT) &&
      valid_text?(book.title) && valid_text?(book.author) &&
      valid_text?(book.genre) && valid_year?(book.year)
  end
end
