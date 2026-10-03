# frozen_string_literal: true

require_relative 'test_helper'

class ValidationTest < Minitest::Test
  def test_valid_text
    assert Validation.valid_text?('Dune')
    refute Validation.valid_text?('')
    refute Validation.valid_text?('   ')
    refute Validation.valid_text?(nil) # dynamic typing: nil must be rejected by hand
    refute Validation.valid_text?(42)
  end

  def test_valid_year_range
    assert Validation.valid_year?(1000)
    assert Validation.valid_year?(Time.now.year)
    refute Validation.valid_year?(999)
    refute Validation.valid_year?(Time.now.year + 1)
    refute Validation.valid_year?('1990')
    refute Validation.valid_year?(nil)
  end

  def test_parse_int_accepts_digits_only
    assert_equal 1990, Validation.parse_int(' 1990 ')
    assert_equal 7, Validation.parse_int('007')
    ['', 'abc', '+5', '-5', '1_000', '19.5', '99999999999999999999', nil].each do |bad|
      assert_nil Validation.parse_int(bad), "expected #{bad.inspect} to be rejected"
    end
  end
end
